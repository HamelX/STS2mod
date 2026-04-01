using GunslingerMod.Models.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Framework.Compatibility;

namespace GunslingerMod.Patches;

// Multiplayer relic-select hand images are loaded from character-specific paths.
// Gunslinger currently has no dedicated hand sprites, so fall back to Silent hands.
[HarmonyPatch(typeof(CharacterModel))]
[HarmonyPatchCategory(GunslingerPatchCategories.CompatibilityVisual)]
public static class CharacterModel_GunslingerHandFallbackPatch
{
    [HarmonyPatch("get_ArmPointingTexture")]
    [HarmonyPostfix]
    private static void ArmPointingTexture_Postfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (__instance is Gunslinger)
            __result = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_point.png"));
    }

    [HarmonyPatch("get_ArmRockTexture")]
    [HarmonyPostfix]
    private static void ArmRockTexture_Postfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (__instance is Gunslinger)
            __result = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_rock.png"));
    }

    [HarmonyPatch("get_ArmPaperTexture")]
    [HarmonyPostfix]
    private static void ArmPaperTexture_Postfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (__instance is Gunslinger)
            __result = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_paper.png"));
    }

    [HarmonyPatch("get_ArmScissorsTexture")]
    [HarmonyPostfix]
    private static void ArmScissorsTexture_Postfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (__instance is Gunslinger)
            __result = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_scissors.png"));
    }
}
