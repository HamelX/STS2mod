# GUNSLINGER PHASE 2A PROGRESS

## What changed
- Added a new BaseLib-aligned framework boundary under `src/GunslingerMod/Framework/` with dedicated bootstrap, registration, compatibility, and localization startup modules.
- Refactored `ModEntry.cs` into a thin initializer delegate that keeps startup logging and forwards startup orchestration to `GunslingerBootstrap.Initialize()`.
- Moved patch activation responsibility out of `ModEntry.cs` and into `GunslingerCompatibilityBootstrap` + `GunslingerRegistration`.
- Introduced explicit Harmony patch category policy (`GunslingerPatchCategories`) so production compatibility patches are activated intentionally while debug patches are not auto-enabled.
- Kept the existing character registration behavior alive by moving the old `ModelDb.AllCharacters` registration patch into a framework registration patch class (`GunslingerCharacterRegistrationPatch`) and applying it through the registration stage.

## What was intentionally NOT changed yet
- No card rebalance and no card text/pool redesign.
- No starter deck changes.
- No gameplay mechanic rewrites for Cylinder / Imprint / Tracer / Seal / Ricochet / Overclock behavior.
- No deterministic replacement for reflection-based character registration yet (the transitional patch remains active for behavior stability).
- No compatibility patch removals for high-risk engine-edge behavior.

## Old patches that still remain active
- `SealedDeck_GunslingerCompatPatch`
- `CardFactory_CreateForMerchantPatch`
- `LargeCapsule_GunslingerCompatPatch`
- `CharacterModel_GunslingerSilentVisualFallbackPatch`
- `CharacterModel_GunslingerHandFallbackPatch`
- `NCombatUi_GunslingerCylinderPatch`
- `NCombatUi_GunslingerCylinderCleanupPatch`
- `NCharacterSelectButtonPatch`
- `NCharacterSelectButtonPressPatch`
- Transitional registration patch: `GunslingerCharacterRegistrationPatch` (migrated from `ModEntry.cs`)

## Debug-only patch policy in this phase
- `NRestSiteRoom_UpgradeDebugPatch` is now categorized as debug-only and is no longer part of default startup activation.

## Next safe step
- Implement and verify a deterministic, non-reflection character registration path through the new framework registration layer.
- After verification (character select + rewards + merchant + events + compendium), remove the transitional `ModelDb.AllCharacters` registration patch and associated private cache invalidation behavior.
