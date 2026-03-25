using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 봉인 폭주
public sealed class SealRampage() : CardModel(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SealEnergyVar()
    ];

    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            if (cylinder == null)
                return false;

            for (var i = 0; i < CylinderPower.MaxRounds; i++)
            {
                if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal && cylinder.GetSealLevel(i) > 0)
                    return true;
            }

            return false;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var hasSeal = false;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal && cylinder.GetSealLevel(i) > 0)
            {
                hasSeal = true;
                break;
            }
        }

        if (!hasSeal)
            return;

        // Recover 1 cost.
        await PlayerCmd.GainEnergy(1, Owner);

        // Sheet spec: halve Seal levels (round down), both base and upgraded.
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var level = cylinder.GetSealLevel(i);
            var targetLevel = level / 2;
            var delta = level - targetLevel;
            if (delta > 0)
                cylinder.ReduceSealLevel(i, (byte)delta);
        }
    }
}
