using System;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Cards;
using GunslingerMod.Models.Powers;
using GunslingerMod.Relics;
using Godot;

namespace GunslingerMod.Models.Combat;

internal static class BulletResolver
{
    public static bool ShouldContinueFiring(CombatState? combat)
        => combat?.HittableEnemies.Any(e => e.IsAlive && e.CurrentHp > 0) == true;

    public static BulletTag GetBulletTag(CylinderPower.AmmoType ammoType)
        => ammoType switch
        {
            CylinderPower.AmmoType.Seal => BulletTag.Seal,
            CylinderPower.AmmoType.Tracer => BulletTag.Tracer,
            _ => BulletTag.None
        };

    public static BulletType GetBulletType(CylinderPower.AmmoType ammoType)
        => GetBulletTag(ammoType) == BulletTag.None ? BulletType.Standard : BulletType.Enhanced;

    public static decimal ResolveBulletEffect(Creature source, CylinderPower.AmmoType ammoType, byte sealLevel, decimal baseDamage)
    {
        var finalDamage = baseDamage;
        var bulletTag = GetBulletTag(ammoType);

        if (bulletTag == BulletTag.Seal)
            finalDamage += source.GetPower<SealRitePower>()?.ConsumeNextSealDamageBonus() ?? 0;

        return finalDamage;
    }

    public static bool TryConsumeCurrentWithSealSkip(
        CylinderPower cylinder,
        CardModel cardSource,
        out CylinderPower.AmmoType ammoType,
        out byte sealLevel)
    {
        var allowSealConsumption = IsSealDedicatedCard(cardSource);
        var allLoadedAreSeal = AreAllLoadedRoundsSeal(cylinder);

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (!cylinder.IsLoaded(cylinder.ChamberIndex))
            {
                var didDryFire = cylinder.TryConsumeCurrent(out ammoType, out sealLevel);
                cylinder.AdvanceChamber();
                return didDryFire;
            }

            var currentType = cylinder.GetAmmoType(cylinder.ChamberIndex);
            if (currentType == CylinderPower.AmmoType.Seal && !allowSealConsumption && !allLoadedAreSeal)
            {
                GD.Print($"[Gunslinger] SealSkip: skipped seal at chamber={cylinder.ChamberIndex} card={cardSource.Id.Entry}");
                cylinder.AdvanceChamber();
                continue;
            }

            var didFire = cylinder.TryConsumeCurrent(out ammoType, out sealLevel);
            cylinder.AdvanceChamber();
            return didFire;
        }

        ammoType = CylinderPower.AmmoType.None;
        sealLevel = 0;
        return false;
    }

    private static bool IsSealDedicatedCard(CardModel cardSource)
        => cardSource is SealReleaseKai or GrandRite or SealSearch or SealBreak;

    private static bool AreAllLoadedRoundsSeal(CylinderPower cylinder)
    {
        var loaded = 0;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (!cylinder.IsLoaded(i))
                continue;

            loaded++;
            if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                return false;
        }

        // Infinite-loop guard should only disable skip when the whole cylinder is Seal.
        return loaded == CylinderPower.MaxRounds;
    }

    public static decimal GetBaseDamage(CylinderPower.AmmoType ammoType, byte sealLevel)
    {
        return ammoType switch
        {
            CylinderPower.AmmoType.Enhanced => 10m,
            CylinderPower.AmmoType.Tracer => 6m,
            CylinderPower.AmmoType.Seal => 10m + sealLevel,
            _ => 8m
        };
    }

    public static ValueProp GetDamageProps(CylinderPower.AmmoType ammoType, byte sealLevel)
    {
        return ammoType == CylinderPower.AmmoType.Penetrator || (ammoType == CylinderPower.AmmoType.Seal && sealLevel >= CylinderPower.SealThresholdUnblockable)
            ? ValueProp.Unblockable | ValueProp.Move
            : ValueProp.Move;
    }

    public static async Task FireAtTarget(
        PlayerChoiceContext choiceContext,
        Creature source,
        Creature target,
        CardModel cardSource,
        CylinderPower.AmmoType ammoType,
        byte sealLevel,
        decimal damage,
        bool suppressTracerTriggers = false)
    {
        if (target == null || !target.IsAlive || target.CurrentHp <= 0)
            return;

        var props = GetDamageProps(ammoType, sealLevel);

        BulletContext.Current = new BulletContext.BulletInfo(ammoType, sealLevel, suppressTracerTriggers);
        try
        {
            if (!HasAliveOpponents(source))
                return;

            var finalDamage = ResolveBulletEffect(source, ammoType, sealLevel, damage);
            finalDamage += source.GetPower<ImprintIgnitionPower>()?.Amount ?? 0;

            // Seal Lv7+: keep a single hit, but double that hit's final damage.
            if (ammoType == CylinderPower.AmmoType.Seal && sealLevel >= CylinderPower.SealThresholdUnblockable)
                finalDamage *= 2m;

            if (!HasAliveOpponents(source))
                return;

            await CreatureCmd.Damage(choiceContext, target, finalDamage, props, source, cardSource);

            if (!HasAliveOpponents(source))
                return;

            await PowerCmd.Apply<ImprintPower>(source, 1, source, cardSource);

            if (!HasAliveOpponents(source))
                return;

            var shotsThisTurn = source.GetPower<ShotsFiredThisTurnPower>();
            if (shotsThisTurn == null)
                shotsThisTurn = await PowerCmd.Apply<ShotsFiredThisTurnPower>(source, 1, source, cardSource);
            else
                shotsThisTurn = await PowerCmd.SetAmount<ShotsFiredThisTurnPower>(source, shotsThisTurn.Amount + 1, source, cardSource);

            var overclockCharge = source.GetPower<OverclockChargePower>()?.Amount ?? 0;
            if (overclockCharge > 0 && shotsThisTurn != null && shotsThisTurn.Amount > 0 && shotsThisTurn.Amount % 3 == 0)
            {
                if (source.Player != null)
                    await PlayerCmd.GainEnergy(overclockCharge, source.Player);

                await PowerCmd.Apply<ImprintPower>(source, 1, source, cardSource);
            }

            if (HotEjectorRelic.CanTriggerFor(source) && source.Player != null)
                await HotEjectorRelic.TriggerFor(source.Player);

            if (!HasAliveOpponents(source))
                return;

            if (ammoType == CylinderPower.AmmoType.Tracer && !suppressTracerTriggers)
            {
                await RegisterTracerShots(choiceContext, source, cardSource, 1);

                var tracerFlag = source.GetPower<TracerFiredThisTurnPower>();
                var isFirstTracerShotThisTurn = tracerFlag == null || tracerFlag.Amount <= 0;
                if (isFirstTracerShotThisTurn)
                    await PowerCmd.Apply<TracerFiredThisTurnPower>(source, 1, source, cardSource);
                else if (tracerFlag != null && tracerFlag.Amount != 1)
                    await PowerCmd.SetAmount<TracerFiredThisTurnPower>(source, 1, source, cardSource);

                if (isFirstTracerShotThisTurn)
                {
                    var ballisticCompiler = source.GetPower<BallisticCompilerPower>()?.Amount ?? 0;
                    if (ballisticCompiler > 0)
                    {
                        if (source.Player != null)
                            await CardPileCmd.Draw(choiceContext, 1, source.Player);

                        await PowerCmd.Apply<ImprintPower>(source, ballisticCompiler, source, cardSource);
                    }

                    if (source.GetPower<OverclockDrumPower>()?.Amount > 0)
                        await TriggerOverclockExtraPull(choiceContext, source, target, cardSource);
                }
            }

        }
        finally
        {
            BulletContext.Current = null;
        }
    }

    public static async Task RegisterTracerShots(PlayerChoiceContext choiceContext, Creature source, CardModel cardSource, int tracerShotsAdded)
    {
        if (tracerShotsAdded <= 0)
            return;

        var drum = source.GetPower<OverclockDrumPower>();
        if (drum == null || drum.Amount <= 0)
            return;

        var explosions = drum.AddTracerShots(tracerShotsAdded);
        if (explosions <= 0)
            return;

        var opponents = source.CombatState?.GetOpponentsOf(source).Where(c => c.IsAlive && c.CurrentHp > 0).ToList();
        if (opponents == null || opponents.Count == 0)
            return;

        const decimal explosionDamage = 8m;
        for (var i = 0; i < explosions; i++)
        {
            foreach (var enemy in opponents.Where(e => e.IsAlive && e.CurrentHp > 0))
                await CreatureCmd.Damage(choiceContext, enemy, explosionDamage, ValueProp.Move, source, cardSource);
        }
    }

    private static async Task TriggerOverclockExtraPull(PlayerChoiceContext choiceContext, Creature source, Creature preferredTarget, CardModel cardSource)
    {
        if (!HasAliveOpponents(source))
            return;

        var cylinder = source.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var target = ResolveAliveTarget(source, preferredTarget);
        if (target == null)
            return;

        var didFire = TryConsumeCurrentWithSealSkip(cylinder, cardSource, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(source, cylinder.CountLoaded(), source, cardSource);

        if (!didFire)
            return;

        if (!HasAliveOpponents(source))
            return;

        var damage = Math.Max(0m, GetBaseDamage(ammoType, sealLevel));
        // Do not allow extra-pull shots to retrigger tracer synergies recursively.
        await FireAtTarget(choiceContext, source, target, cardSource, ammoType, sealLevel, damage, suppressTracerTriggers: true);
    }

    public static bool HasAliveOpponents(Creature source)
        => source.CombatState?.GetOpponentsOf(source).Any(c => c.IsAlive && c.CurrentHp > 0) == true;

    public static Creature? ResolveAliveTarget(Creature source, Creature? preferredTarget)
    {
        if (preferredTarget is { IsAlive: true } && preferredTarget.CurrentHp > 0 && preferredTarget.Side != source.Side)
            return preferredTarget;

        return source.CombatState?.GetOpponentsOf(source).FirstOrDefault(c => c.IsAlive && c.CurrentHp > 0);
    }
}
