using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Framework.Compatibility;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Transitional character registration compatibility patch.
///
/// Phase 2B behavior:
/// - deterministic registration runs first via GunslingerContentRegistrar
/// - this patch remains as a guarded fallback only when deterministic
///   AllCharacters verification fails in the same startup session
///
/// TODO(phase-2c-removal-boundary): delete this patch only after runtime
/// verification confirms character select + reward pools + merchant +
/// compendium/library all function without this fallback.
/// </summary>
[HarmonyPatch(typeof(ModelDb), "AllCharacters", MethodType.Getter)]
[HarmonyPatchCategory(GunslingerPatchCategories.Registration)]
[HarmonyPriority(Priority.First)]
public static class GunslingerCharacterRegistrationPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        if (!GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch)
            return;

        var charactersList = __result.ToList();
        if (charactersList.Any(c => c.GetType() == typeof(Gunslinger)))
            return;

        var gunslingerType = Type.GetType("GunslingerMod.Models.Characters.Gunslinger");
        if (gunslingerType == null)
        {
            GD.PrintErr("[Gunslinger] Gunslinger type not found. Skipping character registration fallback.");
            return;
        }

        var characterMethod = typeof(ModelDb)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "Character" &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 0);

        if (characterMethod == null)
        {
            GD.PrintErr("[Gunslinger] ModelDb.Character<T>() method not found. Skipping character registration fallback.");
            return;
        }

        if (characterMethod.MakeGenericMethod(gunslingerType).Invoke(null, null) is CharacterModel gunslingerCharacter)
            charactersList.Add(gunslingerCharacter);
        else
        {
            GD.PrintErr("[Gunslinger] Failed to instantiate Gunslinger character model fallback.");
            return;
        }

        __result = charactersList;

        // Transitional cache invalidation retained only for fallback mode.
        typeof(ModelDb).GetField("_allCharacterCardPools", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);
        typeof(ModelDb).GetField("_allCards", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }
}
