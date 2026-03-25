using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Characters;

[ModInitializer("Initialize")]
public class ModEntry
{
    public static void Initialize()
    {
        try
        {
            Godot.GD.Print("[Gunslinger] ModEntry.Initialize start");

            var harmony = new Harmony("gunslingermod.patch");
            harmony.PatchAll();

            Godot.GD.Print("[Gunslinger] Harmony.PatchAll complete");
        }
        catch (Exception ex)
        {
            Godot.GD.Print($"[Gunslinger] ModEntry.Initialize exception: {ex}");
        }
    }
}


[HarmonyPatch(typeof(ModelDb), "AllCharacters", MethodType.Getter)]
[HarmonyPriority(Priority.First)]
public class ModelDbAllCharactersPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        var charactersList = __result.ToList();
        charactersList.Add(ModelDb.Character<Gunslinger>());

        __result = charactersList;

        typeof(ModelDb).GetField("_allCharacterCardPools", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);
        typeof(ModelDb).GetField("_allCards", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }
}