using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealSearch() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, IsUpgraded ? 3 : 2, Owner);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.CountSealLoaded() <= 0)
            return;

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

        if (bestIdx < 0)
            return;

        if (bestIdx != cylinder.ChamberIndex)
            cylinder.SwapChambers(bestIdx, cylinder.ChamberIndex);

        // Upgrade reward: the searched seal is immediately refined for next trigger pull.
        if (IsUpgraded && cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
            cylinder.IncrementSealLevel(cylinder.ChamberIndex, 1);
    }

    protected override void OnUpgrade()
    {
    }
}
