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
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        if (BulletResolver.HasAliveOpponents(Owner.Creature))
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

        cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}
