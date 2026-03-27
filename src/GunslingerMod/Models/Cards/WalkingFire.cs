using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class WalkingFire() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IImprintConsumerCard
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= 3;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -3, Owner.Creature, this);

        var pulls = IsUpgraded ? 3 : 2;
        var anyShotSucceeded = false;

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

            anyShotSucceeded = true;
            var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
        }

        if (anyShotSucceeded)
        {
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
        }
    }
}
