using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class BarrageCollapse() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var ricochetStacks = IsUpgraded ? 5 : 4;

        await PowerCmd.Apply<RicochetPower>(Owner.Creature, ricochetStacks, Owner.Creature, this);
        await PowerCmd.Apply<BarrageCollapsePower>(Owner.Creature, ricochetStacks, Owner.Creature, this);
    }
}
