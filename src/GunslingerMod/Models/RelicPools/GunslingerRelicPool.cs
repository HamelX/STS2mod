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
        return
        [
            ModelDb.Relic<CylinderRelic>(),
            ModelDb.Relic<HotEjectorRelic>()
        ];
    }

    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        return AllRelics.ToList();
    }
}
