using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// Random-target full-cylinder barrage. Consumes all loaded rounds.
public sealed class SprayFire() : CardModel(2, CardType.Attack, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SprayFireTotalDamageVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.Amount <= 0)
            return;

        var shotsFired = 0;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (!HasAliveOpponents())
                break;

            var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

            if (!didFire)
                continue;

            var randomTarget = GetDeterministicOpponent(shotsFired + cylinder.ChamberIndex);
            if (randomTarget == null)
                break;

            var shotDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);

            // Per-shot resolution contract:
            // fire once -> apply damage -> re-check combat end before continuing.
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, randomTarget, this, ammoType, sealLevel, shotDamage);
            shotsFired++;

            if (!HasAliveOpponents())
                break;
        }

        if (shotsFired > 0 && HasAliveOpponents())
        {
            var imprintGain = Math.Min(3, shotsFired);
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, imprintGain, Owner.Creature, this);
        }

        bool HasAliveOpponents()
            => BulletResolver.HasAliveOpponents(Owner.Creature);

        Creature? GetDeterministicOpponent(int seed)
        {
            var alive = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Where(c => c.IsAlive && c.CurrentHp > 0).ToList();
            if (alive == null || alive.Count == 0)
                return null;

            // Multiplayer-safe deterministic selection to avoid RNG desync stalls.
            var index = Math.Abs(seed) % alive.Count;
            return alive[index];
        }
    }
}
