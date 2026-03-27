using System;
using GunslingerMod.Models.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Godot;

namespace GunslingerMod.Patches;

/// <summary>
/// Temporarily map Gunslinger visual/animation presentation assets to Silent to reduce
/// missing-resource/null risks until dedicated Gunslinger visuals are finalized.
/// </summary>
[HarmonyPatch(typeof(CharacterModel))]
public static class CharacterModel_GunslingerSilentVisualFallbackPatch
{
    private const string GunslingerCharacterSelectIconPath = "res://images/packed/character_select/char_select_gunslinger.png";
    private const string GunslingerCharacterSelectLockedIconPath = "res://images/packed/character_select/char_select_gunslinger_locked.png";
    private const string GunslingerIconTexturePath = "res://images/ui/top_panel/character_icon_gunslinger.png";
    private const string GunslingerIconOutlineTexturePath = "res://images/ui/top_panel/character_icon_gunslinger_outline.png";
    private const string GunslingerMapMarkerPath = "res://images/packed/map/icons/map_marker_gunslinger.png";

    private static bool IsTarget(CharacterModel model) => model is Gunslinger;

    private static Silent? TryGetSilentModel()
    {
        try
        {
            return ModelDb.Character<Silent>();
        }
        catch
        {
            return null;
        }
    }

    [HarmonyPatch(nameof(CharacterModel.CreateVisuals))]
    [HarmonyPrefix]
    private static bool CreateVisuals_Prefix(CharacterModel __instance, ref NCreatureVisuals __result)
    {
        if (!IsTarget(__instance))
            return true;

        var silent = TryGetSilentModel();
        if (silent is null)
            return true;

        __result = silent.CreateVisuals();
        return false;
    }

    [HarmonyPatch("get_MerchantAnimPath")]
    [HarmonyPostfix]
    private static void MerchantAnimPath_Postfix(CharacterModel __instance, ref string __result)
    {
        if (!IsTarget(__instance))
            return;

        var silent = TryGetSilentModel();
        if (!string.IsNullOrWhiteSpace(silent?.MerchantAnimPath))
            __result = silent.MerchantAnimPath;
    }

    [HarmonyPatch("get_RestSiteAnimPath")]
    [HarmonyPostfix]
    private static void RestSiteAnimPath_Postfix(CharacterModel __instance, ref string __result)
    {
        if (!IsTarget(__instance))
            return;

        var silent = TryGetSilentModel();
        if (!string.IsNullOrWhiteSpace(silent?.RestSiteAnimPath))
            __result = silent.RestSiteAnimPath;
    }

    [HarmonyPatch("get_TrailPath")]
    [HarmonyPostfix]
    private static void TrailPath_Postfix(CharacterModel __instance, ref string __result)
    {
        if (!IsTarget(__instance))
            return;

        var silent = TryGetSilentModel();
        if (!string.IsNullOrWhiteSpace(silent?.TrailPath))
            __result = silent.TrailPath;
    }

    [HarmonyPatch("get_CharacterSelectBg")]
    [HarmonyPostfix]
    private static void CharacterSelectBg_Postfix(CharacterModel __instance, ref string __result)
    {
        if (!IsTarget(__instance))
            return;

        var silent = TryGetSilentModel();
        if (!string.IsNullOrWhiteSpace(silent?.CharacterSelectBg))
            __result = silent.CharacterSelectBg;
    }

    [HarmonyPatch("get_CharacterSelectTransitionPath")]
    [HarmonyPostfix]
    private static void CharacterSelectTransitionPath_Postfix(CharacterModel __instance, ref string __result)
    {
        if (!IsTarget(__instance))
            return;

        var silent = TryGetSilentModel();
        if (!string.IsNullOrWhiteSpace(silent?.CharacterSelectTransitionPath))
            __result = silent.CharacterSelectTransitionPath;
    }

    [HarmonyPatch("get_CharacterSelectIcon")]
    [HarmonyPostfix]
    private static void CharacterSelectIcon_Postfix(CharacterModel __instance, ref CompressedTexture2D __result)
    {
        if (!IsTarget(__instance))
            return;
        __result ??= GD.Load<CompressedTexture2D>(GunslingerCharacterSelectIconPath);
        if (__result != null)
            return;

        var silent = TryGetSilentModel();
        if (silent?.CharacterSelectIcon != null)
            __result = silent.CharacterSelectIcon;
    }

    [HarmonyPatch("get_CharacterSelectLockedIcon")]
    [HarmonyPostfix]
    private static void CharacterSelectLockedIcon_Postfix(CharacterModel __instance, ref CompressedTexture2D __result)
    {
        if (!IsTarget(__instance))
            return;
        __result ??= GD.Load<CompressedTexture2D>(GunslingerCharacterSelectLockedIconPath);
        if (__result != null)
            return;

        var silent = TryGetSilentModel();
        if (silent?.CharacterSelectLockedIcon != null)
            __result = silent.CharacterSelectLockedIcon;
    }

    [HarmonyPatch("get_IconTexture")]
    [HarmonyPostfix]
    private static void IconTexture_Postfix(CharacterModel __instance, ref Texture2D __result)
    {
        if (!IsTarget(__instance))
            return;
        __result ??= GD.Load<Texture2D>(GunslingerIconTexturePath);
        if (__result != null)
            return;

        var silent = TryGetSilentModel();
        if (silent?.IconTexture != null)
            __result = silent.IconTexture;
    }

    [HarmonyPatch("get_IconOutlineTexture")]
    [HarmonyPostfix]
    private static void IconOutlineTexture_Postfix(CharacterModel __instance, ref Texture2D __result)
    {
        if (!IsTarget(__instance))
            return;
        __result ??= GD.Load<Texture2D>(GunslingerIconOutlineTexturePath);
        if (__result != null)
            return;

        var silent = TryGetSilentModel();
        if (silent?.IconOutlineTexture != null)
            __result = silent.IconOutlineTexture;
    }

    [HarmonyPatch("get_MapMarker")]
    [HarmonyPostfix]
    private static void MapMarker_Postfix(CharacterModel __instance, ref CompressedTexture2D __result)
    {
        if (!IsTarget(__instance))
            return;
        __result ??= GD.Load<CompressedTexture2D>(GunslingerMapMarkerPath);
        if (__result != null)
            return;

        var silent = TryGetSilentModel();
        if (silent?.MapMarker != null)
            __result = silent.MapMarker;
    }
}
