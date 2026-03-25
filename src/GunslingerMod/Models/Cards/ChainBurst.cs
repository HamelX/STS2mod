using System;
using System.Linq;
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

        var totalPulls = 1;
        var hasRhythm = (Owner.Creature.GetPower<TracerFiredThisTurnPower>()?.Amount ?? 0) > 0;
        if (hasRhythm)
            totalPulls += IsUpgraded ? 2 : 1;

        for (var i = 0; i < totalPulls; i++)
        {
            if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) != true)
                break;

            var target = cardPlay.Target.IsAlive
                ? cardPlay.Target
                : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);
            if (target == null)
                break;

            var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
            cylinder.AdvanceChamber();
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

            if (!didFire)
                continue;

            var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
        }
    }
}
