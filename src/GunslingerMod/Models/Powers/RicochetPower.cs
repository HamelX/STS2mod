using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;

namespace GunslingerMod.Models.Powers;

// Self-buff ricochet stacks: your bullet hits can bounce while stacks remain.
public sealed class RicochetPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // Trigger from owner's outgoing hits.
        if (Owner == null || dealer != Owner)
            return;

        if (target.IsDead || result.UnblockedDamage <= 0)
            return;

        if (!BulletResolver.HasAliveOpponents(Owner))
            return;

        // Ricochet should only trigger from the original bullet hit, never from ricochet damage.
        if (RicochetContext.IsRicocheting)
            return;

        var bulletInfo = BulletContext.Current;
        var isBulletDamage = bulletInfo != null || BulletContext.IsBulletCardSource(cardSource);
        if (!isBulletDamage)
            return;

        // Per bullet-hit event cap: only one ricochet trigger total.
        if (bulletInfo?.RicochetTriggered == true)
            return;

        // Do not ricochet from Seal shots.
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

        var candidates = combatState.HittableEnemies
            .Where(e => e.IsAlive && !e.IsDead && e.CurrentHp > 0 && e.Side == target.Side && e != target)
            .ToList();
        if (candidates.Count == 0)
            return;

        var bounceTarget = Owner.Player?.RunState.Rng.CombatTargets.NextItem(candidates) ?? candidates[0];
        if (bounceTarget == null || !bounceTarget.IsAlive || bounceTarget.IsDead || bounceTarget.CurrentHp <= 0)
            return;

        if (bulletInfo != null)
            bulletInfo.RicochetTriggered = true;

        await PowerCmd.Decrement(this);
        var barrageCollapse = Owner.GetPower<BarrageCollapsePower>();
        if (barrageCollapse != null && barrageCollapse.Amount > 0 && BulletResolver.HasAliveOpponents(Owner))
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
}
