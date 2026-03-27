using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class QuickRack() : CardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IImprintConsumerCard
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= 1;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        if ((Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0) < 1)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -1, Owner.Creature, this);

        for (var i = 0; i < 2; i++)
            TryLoadTracerWithFallback(cylinder);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        var hasRhythm = (Owner.Creature.GetPower<TracerFiredThisTurnPower>()?.Amount ?? 0) > 0;
        if (!hasRhythm)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
            return;
        }

        var pulls = IsUpgraded ? 2 : 1;
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
    }

    private static bool TryLoadTracerWithFallback(CylinderPower cylinder)
    {
        if (cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer))
            return true;

        // Fallback should be narrow: only retune the current firing chamber when no empty slot exists.
        var idx = cylinder.ChamberIndex;
        var ammo = cylinder.GetAmmoType(idx);
        if (ammo is CylinderPower.AmmoType.Normal or CylinderPower.AmmoType.Enhanced or CylinderPower.AmmoType.Penetrator)
        {
            cylinder.ClearChamberAt(idx);
            return cylinder.TryLoadInto(idx, CylinderPower.AmmoType.Tracer);
        }

        return false;
    }
}
