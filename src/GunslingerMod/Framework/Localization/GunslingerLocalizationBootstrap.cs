using Godot;

namespace GunslingerMod.Framework.Localization;

/// <summary>
/// Localization bootstrap boundary.
///
/// Phase 2A keeps this intentionally lightweight while reserving a dedicated
/// startup stage for future locale key validation and registration plumbing.
/// </summary>
public static class GunslingerLocalizationBootstrap
{
    public static void Apply()
    {
        GD.Print("[Gunslinger] Localization bootstrap phase reached");
    }
}
