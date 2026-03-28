using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GunslingerMod.Models.Powers;

public sealed class BarrageCollapsePower : PowerModel
{
    private const int SplashDamage = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnRicochetTriggered(PlayerChoiceContext choiceContext, Creature ricochetTarget, Creature source, CardModel? cardSource)
    {
        if (Owner == null || Amount <= 0)
            return;

        var combatState = ricochetTarget.CombatState;
        if (combatState == null)
            return;

        await PowerCmd.Decrement(this);

        var splashTargets = combatState.HittableEnemies
            .Where(e => !e.IsDead && e.Side == ricochetTarget.Side && e != ricochetTarget)
            .ToList();

        foreach (var splashTarget in splashTargets)
            await CreatureCmd.Damage(choiceContext, splashTarget, SplashDamage, ValueProp.Move, source, cardSource);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner != null && Owner.Side == side)
            return PowerCmd.SetAmount<BarrageCollapsePower>(Owner, 0, Owner, null);

        return Task.CompletedTask;
    }
}
