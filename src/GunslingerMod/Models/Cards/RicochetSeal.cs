using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class RicochetSeal() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && cylinder.CountSealLoaded() > 0;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || !BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var sealIdx = FindNextSealIndex(cylinder);
        if (sealIdx < 0)
            return;

        if (sealIdx != cylinder.ChamberIndex)
            cylinder.SwapChambers(sealIdx, cylinder.ChamberIndex);

        var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
        if (target == null)
            return;

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);

        if (ammoType == CylinderPower.AmmoType.Seal)
            await PowerCmd.Apply<RicochetPower>(Owner.Creature, IsUpgraded ? 3 : 2, Owner.Creature, this);
    }

    private static int FindNextSealIndex(CylinderPower cylinder)
    {
        if (cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
            return cylinder.ChamberIndex;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
                return i;
        }

        return -1;
    }
}
