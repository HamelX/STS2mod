using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class ImprintSeal() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IImprintConsumerCard
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= (IsUpgraded ? 1 : 2);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var imprintCost = IsUpgraded ? 1 : 2;
        var imprint = Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0;
        if (imprint < imprintCost)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, -imprintCost, Owner.Creature, this);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
                cylinder.IncrementSealLevel(i, (byte)imprintCost);
        }

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}
