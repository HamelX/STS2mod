using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class ChainBurst() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IImprintConsumerCard
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= 2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -2, Owner.Creature, this);

        var remainingPulls = 1;
        var firstResolved = false;
        while (remainingPulls > 0)
        {
            remainingPulls--;
            if (!BulletResolver.ShouldContinueFiring(Owner.Creature.CombatState))
                break;

            var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
            if (target == null)
                break;

            var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

            if (!didFire)
                continue;

            var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);

            if (!firstResolved)
            {
                firstResolved = true;
                if (ammoType == CylinderPower.AmmoType.Tracer)
                    remainingPulls += IsUpgraded ? 3 : 2;
            }
        }
    }
}
