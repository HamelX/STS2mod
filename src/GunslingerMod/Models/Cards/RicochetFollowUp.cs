using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class RicochetFollowUp() : CardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var hasRicochet =
            (Owner.Creature.GetPower<RicochetPower>()?.Amount ?? 0) > 0 ||
            (Owner.Creature.GetPower<RicochetImprintPower>()?.Amount ?? 0) > 0;

        var pulls = 1;
        if (hasRicochet && IsUpgraded)
            pulls += 1;

        for (var i = 0; i < pulls; i++)
        {
            if (!BulletResolver.HasAliveOpponents(Owner.Creature))
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

        if (hasRicochet && !IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }
}
