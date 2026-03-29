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
        // Threshold formula lives in the power: threshold = max(1, 6 - Amount).
        // Base applies 3 stacks (threshold 3+), upgraded applies 4 stacks (threshold 2+).
        await PowerCmd.Apply<ImprintCompressionPower>(Owner.Creature, IsUpgraded ? 4 : 3, Owner.Creature, this);
    }
}
