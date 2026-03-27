using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class EtchedTracer() : CardModel(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool IsPlayable => (Owner?.Creature?.GetPower<CylinderPower>()?.CountLoaded() ?? 0) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.CountLoaded() <= 0)
            return;

        if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) == true)
        {
            var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);

            if (target != null)
            {
                var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);

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
