using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

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

        var sealIdx = FindHighestLevelSealIndex(cylinder);
        if (sealIdx < 0)
            return;

        if (sealIdx != cylinder.ChamberIndex)
            cylinder.SwapChambers(sealIdx, cylinder.ChamberIndex);

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        var combatState = Owner.Creature.CombatState;
        Creature? target = cardPlay.Target;
        for (var i = 0; i < 2; i++)
        {
            if (!BulletResolver.HasAliveOpponents(Owner.Creature))
                return;

            target = GetNextAliveTarget(combatState, Owner.Creature, target);
            if (target == null)
                return;

            var baseDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, baseDamage);

            if (!BulletResolver.HasAliveOpponents(Owner.Creature))
                return;
        }
    }

    private static int FindHighestLevelSealIndex(CylinderPower cylinder)
    {
        var bestIdx = -1;
        var bestLvl = -1;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                continue;

            var lvl = cylinder.GetSealLevel(i);
            if (lvl > bestLvl)
            {
                bestLvl = lvl;
                bestIdx = i;
            }
        }

        return bestIdx;
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
