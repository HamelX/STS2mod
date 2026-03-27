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
        if (cylinder == null || !BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
        if (target == null)
            return;

        // Trigger-pull path: always rotate, only resolve effects if a live round fired.
        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var bulletDamage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, bulletDamage);

        if (!BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        if (!target.IsAlive)
            return;

        var imprint = Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0;
        var perImprint = IsUpgraded ? 2m : 1m;
        var burstDamage = DynamicVars.Damage.BaseValue + (imprint * perImprint);
        if (burstDamage > 0)
            // Non-bullet follow-up hit: use null cardSource so bullet-source fallback checks
            // never misclassify this burst as a bullet when async-local context is unavailable.
            await CreatureCmd.Damage(choiceContext, target, burstDamage, ValueProp.Move, Owner.Creature, null);

        await PowerCmd.Apply<RicochetPower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
