using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SealEnergyVar : EnergyVar
{
    public SealEnergyVar(int energy = 0) : base("Energy", energy)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
        var previewEnergy = 0;

        if (cylinder != null)
        {
            for (var i = 0; i < CylinderPower.MaxRounds; i++)
            {
                if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                    continue;

                var sealLevel = cylinder.GetSealLevel(i);
                var energy = sealLevel >= 15 ? 4 : sealLevel >= 10 ? 3 : sealLevel >= 5 ? 2 : 1;
                if (energy > previewEnergy)
                    previewEnergy = energy;
            }
        }

        PreviewValue = previewEnergy;
        EnchantedValue = previewEnergy;
    }
}
