using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using GunslingerMod.Relics;

namespace GunslingerMod.Models.RelicPools;

public sealed class GunslingerRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "gunslinger";

    public override Color LabOutlineColor => StsColors.gray;

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        // Keep the starter relic registered so character initialization and model lookup remain valid.
        return
        [
            ModelDb.Relic<CylinderRelic>(),
            ModelDb.Relic<HotEjectorRelic>()
        ];
    }

    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        // Starter relics should not appear in random relic rewards / pools.
        return AllRelics.Where(r => r is not CylinderRelic).ToList();
    }
}
