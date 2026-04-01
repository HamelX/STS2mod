using Godot;

namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Placeholder for deterministic BaseLib-aligned content registration ordering.
///
/// In Phase 2A this is intentionally non-invasive and does not alter gameplay
/// content composition yet.
/// </summary>
public static class GunslingerContentRegistrar
{
    public static void Register()
    {
        GD.Print("[Gunslinger] Content registrar phase reached (no gameplay changes in Phase 2A)");
    }
}
