# Gunslinger Phase 2B Verification Fix

## False-positive risk that existed

Phase 2B deterministic verification checked whether `Gunslinger` appeared in `ModelDb.AllCharacters`.

At the same time, the transitional `ModelDb.AllCharacters` fallback patch was enabled by default (`UseTransitionalCharacterRegistrationPatch = true`). Because that patch can inject `Gunslinger` into the getter result, the verification query itself could be contaminated by fallback behavior.

That meant deterministic registration could appear to pass even when deterministic-only registration had not independently succeeded.

## What was changed

A guard was added so deterministic verification executes with fallback patch participation disabled:

1. Registration flow now forces `UseTransitionalCharacterRegistrationPatch = false` before calling deterministic registration/verification.
2. `VerifyCharacterEnumeration()` now also explicitly disables the fallback flag in a local `try/finally` scope while reading `ModelDb.AllCharacters`, restoring the previous value afterward.
3. Fallback activation is still preserved, but is now only enabled *after* deterministic verification fails (`UseTransitionalCharacterRegistrationPatch = !deterministicVerified`).
4. Compatibility patches remain active; this change only gates fallback participation timing.

## Is fallback now truly gated behind deterministic failure?

Yes, in startup flow this is now explicitly gated:

- Verification runs with fallback disabled.
- Only if verification returns `false` does fallback mode become enabled for subsequent runtime usage.

This prevents the verification step from self-fulfilling via the fallback patch.

## What still requires real in-game verification

Repository-only checks cannot fully prove all runtime integration points. The following still need in-game validation:

- Character select visibility.
- Reward pool integration.
- Merchant availability/integration.
- Event interactions.
- Compendium/library listing.

Until those runtime checks are confirmed, retaining fallback behavior as a post-failure safety mechanism remains the safer option.
