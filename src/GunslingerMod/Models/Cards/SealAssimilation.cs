using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealAssimilation() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

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
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var bestIdx = -1;
        var bestLvl = -1;

        if (cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
        {
            bestIdx = cylinder.ChamberIndex;
            bestLvl = cylinder.GetSealLevel(cylinder.ChamberIndex);
        }

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

        // Rebuild pattern by user direction:
        // keep only the chosen highest-level Seal bullet,
        // clear all other chambers, then fill them with temporary Seal bullets at that same level.
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (i == bestIdx)
                continue;

            cylinder.ClearChamberAt(i);
        }

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (i == bestIdx)
                continue;

            cylinder.TryLoadTempSealInto(i, (byte)bestLvl);
        }

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
