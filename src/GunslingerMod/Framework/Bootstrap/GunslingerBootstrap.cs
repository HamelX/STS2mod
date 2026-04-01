using Godot;
using HarmonyLib;
using GunslingerMod.Framework.Compatibility;
using GunslingerMod.Framework.Localization;
using GunslingerMod.Framework.Registration;

namespace GunslingerMod.Framework.Bootstrap;

/// <summary>
/// Framework bootstrap boundary for mod startup orchestration.
///
/// Responsibilities in this phase:
/// - own startup ordering
/// - activate registration and compatibility patch groups
/// - initialize localization bootstrap hooks
///
/// Non-responsibilities in this phase:
/// - gameplay/card balance changes
/// - card pool rewrites
/// </summary>
public static class GunslingerBootstrap
{
    private const string HarmonyId = "gunslingermod.patch";

    public static void Initialize()
    {
        GD.Print("[Gunslinger] Framework bootstrap start");

        var harmony = new Harmony(HarmonyId);

        GunslingerRegistration.Apply(harmony);
        GunslingerCompatibilityBootstrap.Apply(harmony, enableDebugPatches: false);
        GunslingerLocalizationBootstrap.Apply();

        GD.Print("[Gunslinger] Framework bootstrap complete");
    }
}
