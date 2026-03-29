using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

internal static class SealShotHelper
{
    public static int FindHighestLevelSealIndex(CylinderPower cylinder)
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

    public static async Task GrantTemporaryToHand(CardModel source)
    {
        var owner = source.Owner;
        if (owner?.RunState == null)
            return;

        var generated = owner.RunState.CreateCard(ModelDb.Card<SealShot>(), owner);
        await CardPileCmd.Add(generated, PileType.Hand);
    }
}
