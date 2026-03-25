using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class TracerLoad() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        if (cylinder.CountLoaded() == 0)
            cylinder.ResetChambers();

        const int loads = 3;
        for (var i = 0; i < loads; i++)
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }
}
