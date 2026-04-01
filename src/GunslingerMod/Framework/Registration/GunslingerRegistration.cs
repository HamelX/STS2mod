using Godot;
using HarmonyLib;
using GunslingerMod.Framework.Compatibility;

namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Registration phase entrypoint.
///
/// Phase 2B introduces a deterministic registration prewarm path in
/// <see cref="GunslingerContentRegistrar"/>. The transitional
/// AllCharacters compatibility patch remains available and is only used as a
/// fallback when deterministic registration cannot be verified at startup.
/// </summary>
public static class GunslingerRegistration
{
    public static void Apply(Harmony harmony)
    {
        harmony.PatchCategory(typeof(GunslingerRegistration).Assembly, GunslingerPatchCategories.Registration);

        // Prevent fallback-patch participation while deterministic verification runs.
        GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch = false;
        var deterministicVerified = GunslingerContentRegistrar.Register();
        GunslingerRegistrationState.UseTransitionalCharacterRegistrationPatch = !deterministicVerified;

        var mode = deterministicVerified ? "deterministic" : "fallback-patch";
        GD.Print($"[Gunslinger] Registration phase ready (mode={mode})");
    }
}
