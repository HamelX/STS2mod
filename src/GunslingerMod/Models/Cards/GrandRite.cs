using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class GrandRite() : CardModel(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        cylinder.IncrementSealLevels((byte)(IsUpgraded ? 3 : 2));

        var target = cardPlay.Target.IsAlive
            ? cardPlay.Target
            : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);
        if (target == null)
        {
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
            return;
        }

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
    }
}
