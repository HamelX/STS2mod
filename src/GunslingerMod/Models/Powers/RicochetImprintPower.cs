using System;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;

namespace GunslingerMod.Models.Powers;

// Self-buff imprint ricochet stacks.
public sealed class RicochetImprintPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner)
            return;

        if (target.IsDead || result.UnblockedDamage <= 0)
            return;

        // Manifest ricochet should only trigger from the original bullet hit, never from ricochet damage.
        if (RicochetContext.IsRicocheting)
            return;

        var bulletInfo = BulletContext.Current;
        var isBulletDamage = bulletInfo != null || BulletContext.IsBulletCardSource(cardSource);
        if (!isBulletDamage)
            return;

        // Per bullet-hit event cap: only one ricochet trigger total.
        if (bulletInfo?.RicochetTriggered == true)
            return;

        if ((bulletInfo?.AmmoType ?? Models.Powers.CylinderPower.AmmoType.None) == Models.Powers.CylinderPower.AmmoType.Seal)
            return;

        if (Amount <= 0)
            return;

        var unblocked = (decimal)result.UnblockedDamage;
        var ricochetDamage = (int)Math.Floor(unblocked * 0.5m);
        if (ricochetDamage <= 0)
            return;

        var combatState = target.CombatState;
        if (combatState == null)
            return;

        var candidates = combatState.HittableEnemies.Where(e => !e.IsDead && e.Side == target.Side && e != target).ToList();
        var bounceTarget = candidates.Count > 0
            ? (Owner.Player?.RunState.Rng.CombatTargets.NextItem(candidates) ?? candidates[0])
            : target;

        if (bulletInfo != null)
            bulletInfo.RicochetTriggered = true;

        await PowerCmd.Decrement(this);
        var barrageCollapse = Owner.GetPower<BarrageCollapsePower>();
        if (barrageCollapse != null && barrageCollapse.Amount > 0)
            await barrageCollapse.OnRicochetTriggered(choiceContext, bounceTarget, Owner, cardSource);

        var oldContext = RicochetContext.Current;
        RicochetContext.Current = new RicochetContext.RicochetInfo(ricochetDamage);
        try
        {
            if (!BulletResolver.ShouldContinueFiring(Owner.CombatState) || !bounceTarget.IsAlive || bounceTarget.CurrentHp <= 0)
                return;

            await CreatureCmd.Damage(choiceContext, bounceTarget, ricochetDamage, props, dealer, cardSource);
        }
        finally
        {
            RicochetContext.Current = oldContext;
        }
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // Manifest: Ricochet is explicitly a "this turn" effect.
        if (Owner != null && Owner.Side == side)
            return PowerCmd.SetAmount<RicochetImprintPower>(Owner, 0, Owner, null);

        return Task.CompletedTask;
    }
}
