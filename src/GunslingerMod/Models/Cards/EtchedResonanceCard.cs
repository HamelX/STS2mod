using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class EtchedResonance() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var highestLevel = 0;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            highestLevel = Math.Max(highestLevel, cylinder.GetSealLevel(i));
        }

        if (highestLevel <= 0)
            return;

        for (var i = 0; i < highestLevel; i++)
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var current = cylinder.GetSealLevel(i);
            if (current <= (IsUpgraded ? 1 : 0))
                continue;

            cylinder.ReduceSealLevel(i, (byte)(current - (IsUpgraded ? 1 : 0)));
        }

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}
