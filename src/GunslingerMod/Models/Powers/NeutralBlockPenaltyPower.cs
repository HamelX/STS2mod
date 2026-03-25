using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace GunslingerMod.Models.Powers;

// Legacy block-penalty passive (disabled by balance direction).
public sealed class NeutralBlockPenaltyPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldReceiveCombatHooks => true;

    public override decimal ModifyBlockMultiplicative(Creature? target, decimal amount, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        // Disabled: no longer reduce Gunslinger block gain.
        return 1m;
    }
}
