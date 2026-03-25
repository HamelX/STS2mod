using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Patches;

[HarmonyPatch(typeof(NCharacterSelectButton), nameof(NCharacterSelectButton.Init))]
public static class NCharacterSelectButtonPatch
{
    private static void Postfix(NCharacterSelectButton __instance, CharacterModel character)
    {
        if (character is not Gunslinger)
            return;

        // Force unlock for Gunslinger
        AccessTools.Field(typeof(NCharacterSelectButton), "_isLocked")?.SetValue(__instance, false);

        var icon = AccessTools.Field(typeof(NCharacterSelectButton), "_icon")?.GetValue(__instance) as TextureRect;
        if (icon != null)
            icon.Texture = character.CharacterSelectIcon;

        var lockIcon = AccessTools.Field(typeof(NCharacterSelectButton), "_lock")?.GetValue(__instance) as TextureRect;
        if (lockIcon != null)
            lockIcon.Visible = false;
    }
}
