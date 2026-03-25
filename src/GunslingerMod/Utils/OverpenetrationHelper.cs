using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Godot;
using MegaCrit.Sts2.Core.Commands;

namespace GunslingerMod.Utils;

public static class OverpenetrationHelper
{
    // Seal overpenetration: deal the same damage dealt to the primary target to enemies behind it.
    public static async Task Apply(PlayerChoiceContext choiceContext, Creature dealer, Creature primaryTarget, decimal baseDamage, ValueProp props, MegaCrit.Sts2.Core.Models.CardModel? cardSource)
    {
        var combatState = dealer.CombatState;
        if (combatState == null)
            return;

        // Determine ordering by creature node X position; fallback to encounter list ordering.
        List<Creature> aliveOpponents = combatState.GetOpponentsOf(dealer).Where(c => c != null && c.IsAlive).ToList();
        if (aliveOpponents.Count <= 1)
            return;

        float? targetX = null;
        var targetNode = NCombatRoom.Instance?.GetCreatureNode(primaryTarget);
        if (targetNode != null)
            targetX = targetNode.GlobalPosition.X;

        List<Creature> behind;
        if (targetX.HasValue)
        {
            behind = aliveOpponents
                .Where(c => c != primaryTarget)
                .Select(c => (c, node: NCombatRoom.Instance?.GetCreatureNode(c)))
                .Where(t => t.node != null && t.node.GlobalPosition.X > targetX.Value + 0.5f)
                .OrderBy(t => t.node!.GlobalPosition.X)
                .Select(t => t.c)
                .ToList();
        }
        else
        {
            var idx = aliveOpponents.IndexOf(primaryTarget);
            behind = idx >= 0 ? aliveOpponents.Skip(idx + 1).ToList() : new List<Creature>();
        }

        if (behind.Count == 0)
        {
            var idx = aliveOpponents.IndexOf(primaryTarget);
            if (idx >= 0)
                behind = aliveOpponents.Skip(idx + 1).ToList();
        }

        if (behind.Count == 0)
            return;

        var dmg = Math.Max(0m, decimal.Round(baseDamage, 0, MidpointRounding.AwayFromZero));
        if (dmg <= 0)
            return;

        for (var i = 0; i < behind.Count; i++)
        {
            if (dealer.CombatState?.GetOpponentsOf(dealer).Any(c => c.IsAlive) != true)
                break;

            if (!behind[i].IsAlive)
                continue;

            await CreatureCmd.Damage(choiceContext, behind[i], dmg, props, dealer, cardSource);

            if (dealer.CombatState?.GetOpponentsOf(dealer).Any(c => c.IsAlive) != true)
                break;

            // tiny pacing so it reads as multiple hits
            await Cmd.Wait(0.04f);
        }
    }
}
