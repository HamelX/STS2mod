using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealTransfer() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && cylinder.CountSealLoaded() >= 2;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.CountSealLoaded() < 2)
            return;

        var sourceIdx = -1;
        var sourceLvl = -1;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var lvl = cylinder.GetSealLevel(i);
            if (lvl > sourceLvl)
            {
                sourceLvl = lvl;
                sourceIdx = i;
            }
        }

        if (sourceIdx < 0 || sourceLvl <= 0)
            return;

        var targetIdx = -1;
        var lowestLvl = int.MaxValue;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (i == sourceIdx || cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var lvl = cylinder.GetSealLevel(i);
            if (lvl < lowestLvl)
            {
                lowestLvl = lvl;
                targetIdx = i;
            }
        }

        if (targetIdx < 0)
            return;

        var transferAmount = Math.Max(1, sourceLvl / 2);
        cylinder.ReduceSealLevel(sourceIdx, (byte)transferAmount);
        cylinder.IncrementSealLevel(targetIdx, (byte)transferAmount);

        if (IsUpgraded)
            cylinder.IncrementSealLevel(targetIdx, 1);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}