using Godot;
using HarmonyLib;
using GunslingerMod.Framework.Compatibility;

namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Registration phase entrypoint.
///
/// Phase 2A keeps the existing character registration patch behavior alive,
/// but moves activation ownership into the framework layer.
/// </summary>
public static class GunslingerRegistration
{
    public static void Apply(Harmony harmony)
    {
        harmony.PatchCategory(typeof(GunslingerRegistration).Assembly, GunslingerPatchCategories.Registration);
        GunslingerContentRegistrar.Register();
        GD.Print("[Gunslinger] Registration patch group applied");
    }
}
