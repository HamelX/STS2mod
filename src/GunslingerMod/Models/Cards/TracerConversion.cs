using GunslingerMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Cards;

public sealed class TracerConversion() : CardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var converted = 0;
        for (var i = 0; i < CylinderPower.MaxRounds && converted < 2; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Normal)
                continue;

            cylinder.ClearChamberAt(i);
            if (cylinder.TryLoadInto(i, CylinderPower.AmmoType.Tracer))
                converted++;
        }

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}
