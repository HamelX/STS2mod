using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealSearch() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        var drawAmount = IsUpgraded ? 3 : 2;

        if (cylinder != null && cylinder.CountSealLoaded() > 0)
            drawAmount += 1;

        await CardPileCmd.Draw(choiceContext, drawAmount, Owner);
    }

    protected override void OnUpgrade()
    {
    }
}
