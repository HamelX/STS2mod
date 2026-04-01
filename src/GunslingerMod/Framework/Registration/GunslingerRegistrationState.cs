namespace GunslingerMod.Framework.Registration;

/// <summary>
/// Runtime registration mode state shared between deterministic registration and
/// transitional compatibility patch logic.
/// </summary>
public static class GunslingerRegistrationState
{
    public static bool UseTransitionalCharacterRegistrationPatch { get; set; } = false;
}
