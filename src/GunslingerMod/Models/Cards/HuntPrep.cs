using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class HuntPrep() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tracerFlag = Owner.Creature.GetPower<TracerFiredThisTurnPower>();
        if (tracerFlag == null)
            await PowerCmd.Apply<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);
        else if (tracerFlag.Amount != 1)
            await PowerCmd.SetAmount<TracerFiredThisTurnPower>(Owner.Creature, 1, Owner.Creature, this);

        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        if (cylinder.CountLoaded() == 0)
            cylinder.ResetChambers();

        for (var i = 0; i < 2; i++)
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, IsUpgraded ? 2 : 1, Owner);
    }
}
