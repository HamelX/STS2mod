using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealPrimer() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        cylinder.TryLoadNext(CylinderPower.AmmoType.Seal);

        var lowestSealIdx = -1;
        var lowestSealLvl = int.MaxValue;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var level = cylinder.GetSealLevel(i);
            if (level < lowestSealLvl)
            {
                lowestSealLvl = level;
                lowestSealIdx = i;
            }
        }

        if (lowestSealIdx >= 0)
            cylinder.IncrementSealLevel(lowestSealIdx, (byte)(IsUpgraded ? 2 : 1));

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}