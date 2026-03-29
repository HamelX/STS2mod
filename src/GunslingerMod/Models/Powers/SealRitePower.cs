using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

// 遊됱씤 ?섏떇(弱곩뜲?凉?: ??醫낅즺 ??遊됱씤??1諛쒖뿉 異붽?濡??덈꺼+1
public sealed class SealRitePower : PowerModel
{
    public int NextSealDamageBonus { get; private set; }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner == null || Owner.Side != side)
            return;

        var cylinder = Owner.GetPower<CylinderPower>();
        if (cylinder == null)
        {
            cylinder = await PowerCmd.Apply<CylinderPower>(Owner, 1, Owner, null);
            if (cylinder == null)
                return;
        }

        var inc = (byte)(Amount <= 0 ? 1 : Amount);

        // If any seal bullets exist, grow all of them each turn.
        if (cylinder.CountSealLoaded() > 0)
        {
            cylinder.IncrementSealLevels(inc);
            AddNextSealDamageBonus(inc * 3);
        }

        // Removed by balance direction: no auto-load attempt here.
        // Seal Rite now only increases levels of already loaded Seal bullets.
    }

    public void AddNextSealDamageBonus(int amount)
    {
        if (amount <= 0)
            return;

        NextSealDamageBonus += amount;
    }

    public int ConsumeNextSealDamageBonus()
    {
        var bonus = NextSealDamageBonus;
        NextSealDamageBonus = 0;
        return bonus;
    }
}
