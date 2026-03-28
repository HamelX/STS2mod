using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class BlankFire() : CardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tracerFlag = Owner.Creature.GetPower<TracerFiredThisTurnPower>();
        if (tracerFlag == null)
            await PowerCmd.Apply<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);
        else if (tracerFlag.Amount != 1)
            await PowerCmd.SetAmount<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

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

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}
