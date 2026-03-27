using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class GrandChamber() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.CountSealLoaded() <= 0)
            return;

        var sealIdx = -1;
        var sealLevel = -1;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var level = cylinder.GetSealLevel(i);
            if (level > sealLevel)
            {
                sealLevel = level;
                sealIdx = i;
            }
        }

        if (sealIdx < 0)
            return;

        await CardPileCmd.Draw(choiceContext, 2, Owner);

        if (sealIdx != cylinder.ChamberIndex)
            cylinder.SwapChambers(sealIdx, cylinder.ChamberIndex);

        // Modest immediate setup bonus for the aligned Seal release.
        if (cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
            cylinder.IncrementSealLevel(cylinder.ChamberIndex, 1);

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        SetBaseCost(1);
    }
}
