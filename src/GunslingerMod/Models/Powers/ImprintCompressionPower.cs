using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

public sealed class ImprintCompressionPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner.Player != player)
            return;

        var imprint = Owner.GetPower<ImprintPower>()?.Amount ?? 0;
        var threshold = Math.Max(1, 6 - Amount);
        if (imprint < threshold)
            return;

        await CardPileCmd.Draw(choiceContext, 1, player);
    }
}
