using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class BankShot() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var hasRicochet =
            (Owner.Creature.GetPower<RicochetPower>()?.Amount ?? 0) > 0 ||
            (Owner.Creature.GetPower<RicochetImprintPower>()?.Amount ?? 0) > 0;

        if (!BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
        if (target == null)
            return;

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (didFire)
        {
            var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
        }

        if (!hasRicochet)
            return;

        if (TryGetRicochetToConsume(out var powerToConsume))
            await PowerCmd.Decrement(powerToConsume);

        if (target.IsAlive)
            await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    private bool TryGetRicochetToConsume(out PowerModel power)
    {
        var ricochet = Owner.Creature.GetPower<RicochetPower>();
        if (ricochet != null && ricochet.Amount > 0)
        {
            power = ricochet;
            return true;
        }

        var imprintRicochet = Owner.Creature.GetPower<RicochetImprintPower>();
        if (imprintRicochet != null && imprintRicochet.Amount > 0)
        {
            power = imprintRicochet;
            return true;
        }

        power = null!;
        return false;
    }
}
