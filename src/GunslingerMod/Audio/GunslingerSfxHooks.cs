namespace GunslingerMod.Audio;

/// <summary>
/// Placeholder SFX hooks.
///
/// We intentionally keep sound as a late-stage feature, but we want stable call-sites now so
/// Reload/Shoot/Panning/special ammo can all get SFX later without refactors.
///
/// TODO: Implement with the game's audio API once we decide on the exact sound system.
/// </summary>
public static class GunslingerSfxHooks
{
    public static void AmmoInserted(int chamberIndex)
    {
        // Intentionally blank (late-stage).
    }

    public static void ShotFired(int chamberIndex)
    {
        // Intentionally blank (late-stage).
    }

    public static void ShotHit(int chamberIndex)
    {
        // Intentionally blank (late-stage).
    }
}
