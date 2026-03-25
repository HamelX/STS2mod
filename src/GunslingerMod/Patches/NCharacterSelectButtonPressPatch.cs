using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Patches;

[HarmonyPatch(typeof(NCharacterSelectButton), "OnPress")]
public static class NCharacterSelectButtonPressPatch
{
    private static void Postfix(NCharacterSelectButton __instance)
    {
        if (__instance.Character is Gunslinger)
        {
            __instance.Select();
        }
    }
}
