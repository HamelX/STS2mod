using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class Shoot() : CardModel(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || !BulletResolver.ShouldContinueFiring(Owner.Creature.CombatState))
            return;

        var pulls = IsUpgraded ? 2 : 1;
        for (var i = 0; i < pulls; i++)
        {
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
        }
    }
}
