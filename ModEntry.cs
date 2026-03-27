using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

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
        var gunslingerType = Type.GetType("GunslingerMod.Models.Characters.Gunslinger");
        if (gunslingerType == null)
        {
            GD.PrintErr("[Gunslinger] Gunslinger type not found. Skipping character registration.");
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
            GD.PrintErr("[Gunslinger] ModelDb.Character<T>() method not found. Skipping character registration.");
            return;
        }

        if (characterMethod.MakeGenericMethod(gunslingerType).Invoke(null, null) is CharacterModel gunslingerCharacter)
        {
            charactersList.Add(gunslingerCharacter);
        }
        else
        {
            GD.PrintErr("[Gunslinger] Failed to instantiate Gunslinger character model.");
            return;
        }

        __result = charactersList;

        typeof(ModelDb).GetField("_allCharacterCardPools", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);
        typeof(ModelDb).GetField("_allCards", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }
}
