namespace GunslingerMod.Framework.Compatibility;

/// <summary>
/// Harmony patch category identifiers used by Gunslinger startup policy.
///
/// This allows us to keep production compatibility patches active while
/// keeping debug-only patches opt-in.
/// </summary>
public static class GunslingerPatchCategories
{
    public const string Registration = "gunslinger.registration";
    public const string CompatibilityProduction = "gunslinger.compatibility.production";
    public const string CompatibilityVisual = "gunslinger.compatibility.visual";
    public const string CompatibilityUi = "gunslinger.compatibility.ui";
    public const string Debug = "gunslinger.debug";
}
