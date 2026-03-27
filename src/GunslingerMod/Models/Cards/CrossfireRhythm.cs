using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class CrossfireRhythm() : CardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IImprintConsumerCard
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= 2;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var tracerFiredThisTurn = (Owner.Creature.GetPower<TracerFiredThisTurnPower>()?.Amount ?? 0) > 0;
        var triggerPulls = 1 + (tracerFiredThisTurn ? 1 : 0) + (IsUpgraded ? 1 : 0);
        var shotsFired = 0;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -2, Owner.Creature, this);

        for (var i = 0; i < triggerPulls; i++)
        {
            if (!HasAliveOpponents())
                break;

            var target = ResolveAliveTarget(cardPlay.Target);
            if (target == null)
                break;

            var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

            if (!didFire)
            {
                if (!HasAliveOpponents())
                    break;

                continue;
            }

            shotsFired += 1;
            var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);

            if (!HasAliveOpponents())
                break;
        }
        bool HasAliveOpponents()
            => Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) == true;

        MegaCrit.Sts2.Core.Entities.Creatures.Creature? ResolveAliveTarget(MegaCrit.Sts2.Core.Entities.Creatures.Creature preferred)
            => preferred.IsAlive
                ? preferred
                : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);

        // No Imprint gain on this card by latest balance direction.
    }

    protected override void OnUpgrade() { }
}
