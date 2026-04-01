using System.Reflection;
using Godot;
using GunslingerMod.Models.CardPools;
using GunslingerMod.Models.Characters;
using GunslingerMod.Models.Cards;
using GunslingerMod.Models.RelicPools;
using GunslingerMod.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Deterministic framework-level content registration flow.
///
/// This registrar intentionally avoids private-cache invalidation and executes
/// registration in a stable order:
/// 1) character shell and pools
/// 2) card/relic prewarm
/// 3) power prewarm
/// 4) dynamic-var hook probing
/// 5) startup verification of character enumeration
/// </summary>
public static class GunslingerContentRegistrar
{
    private static readonly MethodInfo? ModelDbPowerMethod = ResolveModelDbGenericFactory("Power", typeof(PowerModel));
    private static readonly MethodInfo? ModelDbDynamicVarMethod = ResolveModelDbGenericFactory("DynamicVar", typeof(DynamicVar));

    public static bool Register()
    {
        RegisterCharacterAndPools();
        RegisterCardsAndRelics();
        RegisterPowers();
        RegisterDynamicVars();

        return VerifyCharacterEnumeration();
    }

    private static void RegisterCharacterAndPools()
    {
        _ = ModelDb.Character<Gunslinger>();
        _ = ModelDb.CardPool<GunslingerCardPool>();
        _ = ModelDb.RelicPool<GunslingerRelicPool>();

        GD.Print("[Gunslinger] Deterministic registration: character + pools prewarmed");
    }

    private static void RegisterCardsAndRelics()
    {
        var character = ModelDb.Character<Gunslinger>();

        // Force creation of the full playable pool used by rewards, merchant,
        // and compendium views.
        _ = character.CardPool.AllCards.ToList();
        _ = character.RelicPool.AllRelics.ToList();

        // Ensure non-pooled but runtime-generated support card registration.
        _ = ModelDb.Card<SealShot>();

        // Ensure starter relic stays registered even though filtered from random pools.
        _ = ModelDb.Relic<CylinderRelic>();

        GD.Print("[Gunslinger] Deterministic registration: cards + relics prewarmed");
    }

    private static void RegisterPowers()
    {
        if (ModelDbPowerMethod == null)
        {
            GD.Print("[Gunslinger] Deterministic registration: ModelDb.Power<T>() unavailable, relying on lazy power initialization");
            return;
        }

        var powerTypes = GetConcreteTypes<PowerModel>();
        foreach (var powerType in powerTypes)
        {
            try
            {
                _ = ModelDbPowerMethod.MakeGenericMethod(powerType).Invoke(null, null);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[Gunslinger] Failed power prewarm for {powerType.FullName}: {ex.Message}");
            }
        }

        GD.Print($"[Gunslinger] Deterministic registration: powers prewarmed ({powerTypes.Count})");
    }

    private static void RegisterDynamicVars()
    {
        var dynamicVarTypes = GetConcreteTypes<DynamicVar>();

        if (ModelDbDynamicVarMethod != null)
        {
            foreach (var dynamicVarType in dynamicVarTypes)
            {
                try
                {
                    _ = ModelDbDynamicVarMethod.MakeGenericMethod(dynamicVarType).Invoke(null, null);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[Gunslinger] Failed dynamic-var prewarm for {dynamicVarType.FullName}: {ex.Message}");
                }
            }

            GD.Print($"[Gunslinger] Deterministic registration: dynamic vars prewarmed via ModelDb ({dynamicVarTypes.Count})");
            return;
        }

        GD.Print($"[Gunslinger] Deterministic registration: ModelDb.DynamicVar<T>() unavailable, dynamic vars rely on card canonical registration ({dynamicVarTypes.Count})");
    }

    private static bool VerifyCharacterEnumeration()
    {
        // Safety guard: never allow transitional fallback patch to participate in
        // deterministic verification itself.
        var previousFallbackMode = GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch;
        GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch = false;

        try
        {
            var hasGunslinger = ModelDb.AllCharacters.Any(c => c.GetType() == typeof(Gunslinger));
            if (!hasGunslinger)
            {
                GD.PrintErr("[Gunslinger] Deterministic registration verification failed: Gunslinger not in ModelDb.AllCharacters");
                return false;
            }

            GD.Print("[Gunslinger] Deterministic registration verification passed: Gunslinger appears in ModelDb.AllCharacters");
            return true;
        }
        finally
        {
            GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch = previousFallbackMode;
        }
    }

    private static List<Type> GetConcreteTypes<TBase>()
    {
        return typeof(GunslingerContentRegistrar)
            .Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(TBase).IsAssignableFrom(t))
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .ToList();
    }

    private static MethodInfo? ResolveModelDbGenericFactory(string methodName, Type expectedBaseType)
    {
        return typeof(ModelDb)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == methodName &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 0 &&
                expectedBaseType.IsAssignableFrom(m.ReturnType));
    }
}
