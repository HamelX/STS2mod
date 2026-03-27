using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class HuntReload() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        if (cylinder.CountLoaded() == 0)
            cylinder.ResetChambers();

        cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        var hasRhythm = (Owner.Creature.GetPower<TracerFiredThisTurnPower>()?.Amount ?? 0) > 0;
        if (hasRhythm)
        {
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);
            if (IsUpgraded)
                await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }
}
