using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class ReadTheMark() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);

        var hasRicochetStack =
            (Owner.Creature.GetPower<RicochetPower>()?.Amount ?? 0) > 0 ||
            (Owner.Creature.GetPower<RicochetImprintPower>()?.Amount ?? 0) > 0;

        if (hasRicochetStack)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
