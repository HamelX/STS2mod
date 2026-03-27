using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealImprint() : CardModel(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var minLevel = IsUpgraded ? 7 : 5;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var level = cylinder.GetSealLevel(i);
            if (level < minLevel)
                cylinder.IncrementSealLevel(i, (byte)(minLevel - level));

            break;
        }

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }
}
