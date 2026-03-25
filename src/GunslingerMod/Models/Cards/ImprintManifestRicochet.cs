using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class ImprintManifestRicochet() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private const int TurnWideRicochetStacks = 99;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int ImprintCostBase = 4;
    private const int ImprintCostUpgraded = 3;

    protected override bool IsPlayable
    {
        get
        {
            var imprint = Owner?.Creature?.GetPower<ImprintPower>();
            var cost = IsUpgraded ? ImprintCostUpgraded : ImprintCostBase;
            return imprint != null && imprint.Amount >= cost;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cost = IsUpgraded ? ImprintCostUpgraded : ImprintCostBase;
        if ((Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0) < cost)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -cost, Owner.Creature, this);
        await PowerCmd.Apply<RicochetImprintPower>(Owner.Creature, TurnWideRicochetStacks, Owner.Creature, this);
    }
}
