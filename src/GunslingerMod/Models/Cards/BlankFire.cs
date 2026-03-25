using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class BlankFire() : CardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.None), IImprintConsumerCard
{
    protected override bool IsPlayable
    {
        get
        {
            var cost = IsUpgraded ? 1 : 2;
            return (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= cost;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cost = IsUpgraded ? 1 : 2;
        if ((Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0) < cost)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -cost, Owner.Creature, this);

        var tracerFlag = Owner.Creature.GetPower<TracerFiredThisTurnPower>();
        if (tracerFlag == null)
            await PowerCmd.Apply<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);
        else if (tracerFlag.Amount != 1)
            await PowerCmd.SetAmount<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) != true)
            return;

        var target = cardPlay.Target?.IsAlive == true
            ? cardPlay.Target
            : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);
        if (target == null)
            return;

        var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
        cylinder.AdvanceChamber();
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
    }
}
