using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 봉인해제 : 개
public sealed class SealReleaseKai() : CardModel(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && cylinder.CountSealLoaded() > 0;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var sealIdx = FindNextSealIndex(cylinder);
        if (sealIdx < 0)
            return;

        if (sealIdx != cylinder.ChamberIndex)
            cylinder.SwapChambers(sealIdx, cylinder.ChamberIndex);

        var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
        cylinder.AdvanceChamber();
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var combatState = Owner.Creature.CombatState;
        Creature? target = cardPlay.Target;
        var shotsFired = 0;

        // Fire the consumed Seal bullet twice.
        for (var i = 0; i < 2; i++)
        {
            if (!BulletResolver.HasAliveOpponents(Owner.Creature))
                return;

            target = GetNextAliveTarget(combatState, Owner.Creature, target);
            if (target == null)
                return;

            var baseDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, baseDamage);
            shotsFired++;

            if (!BulletResolver.HasAliveOpponents(Owner.Creature))
                return;
        }

        if (shotsFired > 0 && BulletResolver.HasAliveOpponents(Owner.Creature))
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, Math.Min(3, shotsFired), Owner.Creature, this);
    }

    private static int FindNextSealIndex(CylinderPower cylinder)
    {
        if (cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
            return cylinder.ChamberIndex;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
                return i;
        }

        return -1;
    }

    private static Creature? GetNextAliveTarget(CombatState? combatState, Creature owner, Creature? currentTarget)
    {
        if (currentTarget?.IsAlive == true)
            return currentTarget;

        var enemies = combatState?.GetOpponentsOf(owner);
        if (enemies == null)
            return null;

        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
                return enemy;
        }
        return null;
    }
}
