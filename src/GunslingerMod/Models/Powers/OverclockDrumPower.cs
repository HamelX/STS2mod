using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

public sealed class OverclockDrumPower : PowerModel
{
    public int TracerShotCounter { get; private set; }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public int AddTracerShots(int amount)
    {
        if (amount <= 0)
            return 0;

        TracerShotCounter += amount;
        var explosionCount = TracerShotCounter / 5;
        if (explosionCount > 0)
            TracerShotCounter %= 5;

        return explosionCount;
    }
}
