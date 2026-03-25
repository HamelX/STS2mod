using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class RicochetShot() : CardModel(3, CardType.Skill, CardRarity.Rare, TargetType.None), IImprintConsumerCard
{
    private const int ImprintCost = 3;

    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= ImprintCost;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if ((Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0) < ImprintCost)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -ImprintCost, Owner.Creature, this);
        await PowerCmd.Apply<RicochetPower>(Owner.Creature, 2, Owner.Creature, this);
        await PowerCmd.Apply<NextAttackFreePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
