using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealRampage() : CardModel(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && FindHighestLevelSealIndex(cylinder) >= 0;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var sealIndex = FindHighestLevelSealIndex(cylinder);
        if (sealIndex < 0)
            return;

        var reduction = IsUpgraded ? 1 : 2;
        var currentLevel = cylinder.GetSealLevel(sealIndex);
        var delta = Math.Min(currentLevel, reduction);
        if (delta > 0)
            cylinder.ReduceSealLevel(sealIndex, (byte)delta);

        await PlayerCmd.GainEnergy(1, Owner);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    private static int FindHighestLevelSealIndex(CylinderPower cylinder)
    {
        var bestIdx = -1;
        var bestLvl = -1;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var lvl = cylinder.GetSealLevel(i);
            if (lvl > bestLvl)
            {
                bestLvl = lvl;
                bestIdx = i;
            }
        }

        return bestIdx;
    }
}
