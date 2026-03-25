using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class TagBurst() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new ImprintDamageVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        // Trigger-pull path: always rotate, only resolve effects if a live round fired.
        var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
        cylinder.AdvanceChamber();
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var bulletDamage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, cardPlay.Target, this, ammoType, sealLevel, bulletDamage);
        await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);

        if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) != true)
            return;

        if (!cardPlay.Target.IsAlive)
            return;

        var imprint = Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0;
        var perImprint = IsUpgraded ? 2m : 1m;
        var burstDamage = DynamicVars.Damage.BaseValue + (imprint * perImprint);
        if (burstDamage > 0)
            // Non-bullet follow-up hit: use null cardSource so bullet-source fallback checks
            // never misclassify this burst as a bullet when async-local context is unavailable.
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, burstDamage, ValueProp.Move, Owner.Creature, null);

        await PowerCmd.Apply<RicochetPower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
