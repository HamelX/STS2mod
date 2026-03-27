using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class OverclockDrum() : CardModel(2, CardType.Power, CardRarity.Rare, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var existing = Owner.Creature.GetPower<OverclockDrumPower>();
        if (existing?.Amount > 0)
            return;

        await PowerCmd.Apply<OverclockDrumPower>(Owner.Creature, 1, Owner.Creature, this);
    }
}
