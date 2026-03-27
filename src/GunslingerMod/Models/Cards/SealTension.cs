using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealTension() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var sealIdx = -1;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
            {
                sealIdx = i;
                break;
            }
        }

        if (sealIdx >= 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, IsUpgraded ? 10m : 7m, ValueProp.Move, cardPlay);
            cylinder.IncrementSealLevel(sealIdx, (byte)(IsUpgraded ? 3 : 2));
        }
        else
        {
            cylinder.TryLoadNext(CylinderPower.AmmoType.Seal);
        }

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }
}
