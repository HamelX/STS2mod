using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;

namespace GunslingerMod.Models.Powers;

/// <summary>
/// Evasion+: base Evasion (halve damage this turn) + flat damage reduction.
/// Intended upgrade behavior per design discussion.
/// </summary>
public sealed class EvasionPlusPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldReceiveCombatHooks => true;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner)
            return 0.5m;
        return 1m;
    }

    // NOTE: We don't know the exact name of the core flat-damage hook from the decompiled sources.
    // This method compiles only if the core exposes it on PowerModel.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner)
            return -1m;
        return 0m;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // Keep Evasion+ through owner's turn so it mitigates incoming enemy attacks.
        // Expire after the opposing side finishes their turn.
        if (Owner != null && Owner.Side != side)
            return PowerCmd.SetAmount<EvasionPlusPower>(Owner, 0, Owner, null);

        return Task.CompletedTask;
    }
}
