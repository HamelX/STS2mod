using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

public sealed class TracerFiredThisTurnPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
            return PowerCmd.SetAmount<TracerFiredThisTurnPower>(Owner, 0, Owner, null);

        return Task.CompletedTask;
    }
}
