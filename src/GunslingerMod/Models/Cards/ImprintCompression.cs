using GunslingerMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Cards;

public sealed class ImprintCompression() : CardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Threshold formula lives in the power: threshold = max(1, 5 - Amount).
        // Base applies 1 stack (threshold 4+), upgraded applies 2 stacks (threshold 3+).
        await PowerCmd.Apply<ImprintCompressionPower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }
}
