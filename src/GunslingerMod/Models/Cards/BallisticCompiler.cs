using GunslingerMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Cards;

public sealed class BallisticCompiler() : CardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // First Tracer shot each turn: draw 1 + gain Imprint equal to stacks.
        // Base: +1 stack, upgraded: +2 stacks.
        await PowerCmd.Apply<BallisticCompilerPower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }
}
