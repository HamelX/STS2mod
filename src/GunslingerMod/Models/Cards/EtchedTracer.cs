using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class EtchedTracer() : CardModel(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<CylinderPower>()?.CountLoaded() ?? 0) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.CountLoaded() <= 0)
            return;

        if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) == true)
        {
            var target = cardPlay.Target?.IsAlive == true
                ? cardPlay.Target
                : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);

            if (target != null)
            {
                var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
                cylinder.AdvanceChamber();

                if (didFire)
                {
                    var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
                    await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
                }
            }
        }

        var tracerLoads = IsUpgraded ? 2 : 1;
        for (var i = 0; i < tracerLoads; i++)
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}
