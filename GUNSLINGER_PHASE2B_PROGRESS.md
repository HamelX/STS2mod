# GUNSLINGER PHASE 2B PROGRESS

## Deterministic registration path implemented

Implemented a deterministic framework registration flow in `GunslingerContentRegistrar` and wired it into `GunslingerRegistration`.

Startup registration order now is:
1. Register/pregenerate character + pools:
   - `ModelDb.Character<Gunslinger>()`
   - `ModelDb.CardPool<GunslingerCardPool>()`
   - `ModelDb.RelicPool<GunslingerRelicPool>()`
2. Prewarm content used by runtime systems and compendium/library-relevant model enumeration:
   - force `character.CardPool.AllCards`
   - force `character.RelicPool.AllRelics`
   - pre-register generated support card `SealShot`
   - pre-register starter relic model `CylinderRelic`
3. Prewarm Gunslinger powers by scanning concrete `PowerModel` types and calling `ModelDb.Power<T>()` when available.
4. Probe dynamic-var registration path:
   - if `ModelDb.DynamicVar<T>()` exists, prewarm all concrete `DynamicVar` types
   - otherwise log and rely on canonical card var registration path
5. Verify deterministic registration by confirming `Gunslinger` appears in `ModelDb.AllCharacters`.

A registration mode state (`GunslingerRegistrationState`) is now set based on verification result.

## Transitional reflection patch status

`GunslingerCharacterRegistrationPatch` was **retained** in this phase, but downgraded to a guarded fallback.

- If deterministic verification passes, the transitional patch is inert (`UseTransitionalCharacterRegistrationPatch = false`).
- If deterministic verification fails, the patch remains active and continues legacy reflection + private cache invalidation as a safety fallback.

This keeps runtime behavior stable while preventing speculative hard removal.

## Verification performed in this change set

Performed code-level and startup-flow verification in repository context:
- deterministic registration stage is invoked before compatibility/bootstrap completion
- deterministic registrar now prewarms:
  - character
  - card pool
  - relic pool
  - powers
  - dynamic vars (conditionally)
- fallback patch remains available and scoped to failure mode only
- required compatibility patches remain active (merchant, SealedDeck, LargeCapsule, visual/UI patches unchanged)

Runtime UI/gameplay verification points (character select visibility, reward rolls, merchant generation, compendium/library interactions) cannot be fully executed in this repo-only environment.

## Remaining blockers for full removal of reflection fallback

Full runtime confirmation is still required in an actual game session for:
- character select appearance without fallback patch participation
- reward/card pool behavior across runs
- relic pool behavior and starter relic exclusion correctness
- merchant generation behavior in low/empty bucket scenarios
- compendium/library visibility stability

Until those are confirmed, removing the transitional patch would be unsafe.

## Next safe step

Run in-game verification with deterministic mode enabled and confirm all of the following in one tested build:
1. Gunslinger appears in character select.
2. Card rewards draw from Gunslinger card pool.
3. Relic rewards draw from Gunslinger relic pool with starter relic excluded.
4. Merchant card generation remains stable.
5. Compendium/library include Gunslinger content correctly.

If all pass, remove `GunslingerCharacterRegistrationPatch` and the registration mode fallback state in the same change set.
