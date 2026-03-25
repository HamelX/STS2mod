using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

/// <summary>
/// Makes the owner's next Attack card cost 0 in combat, then removes itself.
/// </summary>
public sealed class NextAttackFreePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Type != CardType.Attack)
            return false;

        if (card.Owner?.Creature != Owner)
            return false;

        modifiedCost = 0;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        return PowerCmd.Remove<NextAttackFreePower>(Owner);
    }
}
