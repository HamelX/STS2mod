using Godot;
using HarmonyLib;

namespace GunslingerMod.Framework.Compatibility;

/// <summary>
/// Compatibility patch activation policy.
///
/// Responsibilities:
/// - activate production compatibility patches needed for runtime safety
/// - activate visual/UI fallback patches needed for stable presentation
/// - keep debug/temporary patches opt-in
///
/// Gameplay logic remains in gameplay model/card/power files and is not changed here.
/// </summary>
public static class GunslingerCompatibilityBootstrap
{
    public static void Apply(Harmony harmony, bool enableDebugPatches)
    {
        var assembly = typeof(GunslingerCompatibilityBootstrap).Assembly;

        harmony.PatchCategory(assembly, GunslingerPatchCategories.CompatibilityProduction);
        harmony.PatchCategory(assembly, GunslingerPatchCategories.CompatibilityVisual);
        harmony.PatchCategory(assembly, GunslingerPatchCategories.CompatibilityUi);

        if (enableDebugPatches)
            harmony.PatchCategory(assembly, GunslingerPatchCategories.Debug);

        GD.Print($"[Gunslinger] Compatibility patch groups applied (debug={enableDebugPatches})");
    }
}
