# GUNSLINGER_BASELIB_STRUCTURE_MAPPING

This document is a concrete phase-1 architecture mapping pass that uses:
- the current Gunslinger repository state,
- the existing `GUNSLINGER_BASELIB_MIGRATION_MASTERPLAN.md`,
- and the publicly visible BaseLib/ModTemplate-StS2 layout signals (BaseLib repo folder layout + ModTemplate character-template conventions referenced from BaseLib and template wiki).

It is intentionally **mapping only** (no mass edits, no card rewrites, no balance changes).

## Expected BaseLib-oriented structure

Expected target shape for a BaseLib-oriented **character mod** (adapted from BaseLib + ModTemplate character template conventions):

- Thin `ModEntry` / initializer that delegates to explicit bootstrap phases.
- A dedicated framework/integration boundary (registration, compatibility, localization wiring).
- A gameplay boundary (cards, powers, relic logic, combat helpers) that is independent from bootstrapping.
- Explicit dependency contract (`GunslingerMod.json`) plus build-time dependency clarity (`.csproj` package/reference layout).
- Harmony patches limited to real compatibility gaps, not primary registration.

### Target directory intent

```text
ModEntry.cs
src/GunslingerMod/
  Framework/
    Bootstrap/
      GunslingerBootstrap.cs
    Registration/
      GunslingerRegistration.cs
      GunslingerContentRegistrar.cs
    Localization/
      GunslingerLocalizationBootstrap.cs
    Compatibility/
      MerchantCompatPatch.cs
      SealedDeckCompatPatch.cs
      LargeCapsuleCompatPatch.cs
  Gameplay/
    Characters/
    CardPools/
    RelicPools/
    Cards/
    Powers/
    Combat/
    DynamicVars/
    Relics/
  UI/
    GunslingerCylinderUi...
```

(Names above are target-aligned placeholders chosen to fit this repository’s current layout and naming.)

## Current Gunslinger structure

Current repository layout is functional but registration/integration-centric behavior is still concentrated in `ModEntry.cs` + broad `PatchAll()` + mixed-purpose `Patches/`.

- Initialization: `ModEntry.Initialize()` does global Harmony patching.
- Character registration: injected via Harmony patch on `ModelDb.AllCharacters` getter using reflection and cache invalidation.
- Content models: mostly cleanly separated under `src/GunslingerMod/Models/*`.
- Relics: separate `src/GunslingerMod/Relics/*`.
- Custom UI node + combat UI hooks: `src/GunslingerMod/Nodes/*` + `NCombatUi_*` patches.
- Compatibility patches and temporary/debug patches are colocated in `src/GunslingerMod/Patches/*`.
- Dependency metadata split between `GunslingerMod.json` and generated `mod_manifest.json`.
- Build references use local game DLL hint paths in `GunslingerMod.csproj`.

## Exact divergences

### 1) ModEntry / initialization

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `ModEntry.cs` (`Initialize` + `Harmony.PatchAll`) | Global startup and all patch activation | `ModEntry.cs` calling `Framework/Bootstrap/GunslingerBootstrap.Initialize()` with staged boot (framework check -> registration -> compat patch activation) | rewrite | Reduce patch-first startup and make ordering deterministic | Medium (startup regressions if ordering wrong) | Requires BaseLib presence check before registration stage |
| `Harmony.PatchAll()` blanket behavior | Activates both essential and temporary patches | Explicit `PatchCategory`/targeted patch registration in Framework compatibility bootstrap | rewrite | Avoid unintentional activation of debug/legacy patches | Low-Medium | Keep Harmony runtime ref; BaseLib remains runtime mod dependency |

### 2) Character registration

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `ModEntry.cs` + `ModelDbAllCharactersPatch` postfix | Inject Gunslinger into `ModelDb.AllCharacters` via reflection | `Framework/Registration/GunslingerRegistration.cs` explicit character registration call during startup | delete (patch), add/rewrite registration module | Current reflection/cache reset approach is brittle vs engine updates | High (character may disappear from select/pools if replacement incomplete) | Depends on BaseLib-supported registration lifecycle |
| Reflection calls (`Type.GetType`, `MakeGenericMethod`, private fields `_allCharacterCardPools`, `_allCards`) | Late binding and cache invalidation side effects | No private reflection for registration; use declared registration API path | delete | Eliminate private cache coupling | High | Requires verified BaseLib/API path to refresh/seed registries safely |

### 3) Card pool registration

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `src/GunslingerMod/Models/CardPools/GunslingerCardPool.cs` | Canonical Gunslinger card pool composition | Keep gameplay pool model, but register through `Framework/Registration/GunslingerContentRegistrar` | keep + move boundary responsibility | Pool content is correct location; boot responsibility should move out of patches | Medium | Registration should run after character registration shell is established |
| Implicit registration through `ModelDb.Card<T>()` calls occurring via usage | Runtime discovery via access paths | Explicit staged registration of all card models/dynamic vars used by pool | rewrite (registration path) | Make registration deterministic for compendium/rewards/events | Medium-High | Dynamic vars and generated cards (`SealShot`) must be included |

### 4) Relic pool registration

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `src/GunslingerMod/Models/RelicPools/GunslingerRelicPool.cs` | Defines relic pool + starter relic filtering (`CylinderRelic` excluded from random) | Keep model, register through Framework registration phase | keep + move boundary responsibility | Logic is good and character-specific | Low-Medium | Must remain aligned with `Gunslinger.StartingRelics` |
| `GetUnlockedRelics` filtering starter relic | Prevent starter relic in random rewards | Keep as custom gameplay rule | keep | This is character design, not framework concern | Low | Needs regression check with reward generation |

### 5) Power / hook / patch boundaries

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `src/GunslingerMod/Models/Powers/*` + gameplay card/power logic | Core Gunslinger mechanics | Keep under gameplay boundary (optionally rename `Models` -> `Gameplay`) | keep | Domain logic already mostly clean | Low | No BaseLib rewrite needed for mechanics itself |
| `src/GunslingerMod/Patches/NRestSiteRoom_UpgradeDebugPatch.cs` | Temporary diagnostic logging | Remove from production activation path (or delete after validation) | delete earliest | Debug-only patch broadens runtime patch surface | Low | None |
| `src/GunslingerMod/Patches/NCharacterSelectButtonPatch.cs` and `NCharacterSelectButtonPressPatch.cs` | Force unlock / force-select behavior | Keep only if still needed after proper registration/unlock handling; otherwise remove | rewrite or delete | These are likely workaround patches rather than stable integration contracts | Medium | Must verify unlock flow under BaseLib-oriented registration |
| `src/GunslingerMod/Patches/NCombatUi_GunslingerCylinderPatch.cs` + cleanup patch | Inject/remove custom cylinder UI node | Keep custom behavior, move to `Framework/Compatibility/UI` and gate strictly to Gunslinger | keep + move | This is custom UX, likely still needed post-migration | Medium | Depends on combat node lifecycle hooks remaining stable |

### 6) Manifest / dependency metadata

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `GunslingerMod.json` (`dependencies: ["BaseLib"]`) | Runtime mod dependency contract | Keep as dependency source of truth; add minimum tested BaseLib version policy in docs/release checklist | keep + rewrite docs process | Aligns with BaseLib dependency model | Low | BaseLib currently declared but version policy unspecified |
| `mod_manifest.json` generated from csproj target | Engine packaging metadata | Keep for package metadata only; do not duplicate dependency authority here | keep | Avoid metadata drift/confusion | Low | Must ship alongside `.dll`, `.pck`, `GunslingerMod.json` |

### 7) csproj dependency structure

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `GunslingerMod.csproj` direct refs (`sts2.dll`, `GodotSharp.dll`, `0Harmony.dll`) | Compile against local game runtime assemblies | Keep engine refs; add explicit BaseLib dependency strategy (NuGet `PackageReference` if available or documented runtime dependency-only policy) | rewrite (dependency clarity, not mass build surgery) | Clarifies compile/runtime contracts and reduces hidden coupling | Medium | Current local props pathing remains required |
| `GenerateModManifest` target in csproj | Build-time manifest generation | Keep with documented scope limitation | keep | Useful packaging automation | Low | Ensure no dependency duplication expectations |

### 8) Localization / compendium / tooltip registration

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `GunslingerMod/localization/{eng,kor,jpn}/*.json` | Strings for cards/powers/relics/characters/static tooltips | Keep files; add framework localization bootstrap/validation pass | keep + add validation bootstrap | Current asset content exists but no explicit startup validation stage | Medium | `NGunslingerCylinderUi` consumes `static_hover_tips` keys directly |
| `src/GunslingerMod/Nodes/NGunslingerCylinderUi.cs` LocString calls (`static_hover_tips`) | In-combat tooltip assembly for chamber/ammo | Keep custom tooltip logic; add key existence checks in migration validation | keep | Character-specific UI semantics should remain custom | Low-Medium | Missing keys would fail UX silently; validate per locale |
| Compendium visibility currently implicit via pool and compatibility patches | Determine what appears/rolls | Move to explicit registration + compatibility adapter ownership | rewrite (ownership) | Reduce accidental pool drift or patch side-effects | Medium-High | Tied to reward/merchant/SealedDeck behavior |

### 9) Compatibility patches

| current_file_or_pattern | current_purpose | BaseLib-aligned_target | keep / move / rewrite / delete | why | migration_risk | dependency_notes |
|---|---|---|---|---|---|---|
| `CardFactory_CreateForMerchantPatch.cs` | Merchant fallback when rarity/type bucket empty | Keep (at least interim), relocate under Framework/Compatibility, retain Gunslinger guard | keep | Protects against merchant generation crash conditions | Medium | Depends on card pool size/distribution after migration |
| `SealedDeck_GunslingerCompatPatch.cs` | Clamp candidate/pick counts to prevent exhaustion crash | Keep until replaced by verified shared compatibility abstraction | keep (must remain until verified) | Known crash-prevention logic for small pools | High if removed early | Critical until reward generation path is proven safe |
| `LargeCapsule_GunslingerCompatPatch.cs` | Safe strike/defend fallback for event relic additions | Keep, maybe simplify once starter/basic tags are guaranteed | keep | Prevents event softlocks with nonstandard basics | Medium | Depends on card tags and starter composition |
| `CharacterModel_GunslingerSilentVisualFallbackPatch.cs` / hand fallback patch | Visual fallback to Silent assets for missing Gunslinger-specific assets | Keep short-term, mark as temporary compatibility layer | keep (temporary) | Prevents null/missing visual issues | Low-Medium | Remove only when Gunslinger visuals complete |

## Replacement plan

1. **Create framework boundary first (no gameplay rewrites):**
   - Add `src/GunslingerMod/Framework/Bootstrap/GunslingerBootstrap.cs`.
   - Add `src/GunslingerMod/Framework/Registration/GunslingerRegistration.cs`.
   - Add `src/GunslingerMod/Framework/Registration/GunslingerContentRegistrar.cs`.
   - Move patch activation policy into `Framework/Compatibility/*`.

2. **Refactor startup path:**
   - `ModEntry.Initialize()` should do logging + call a single bootstrap entry.
   - Replace blanket patch-all behavior with explicit compatibility patch registration groups.

3. **Replace character injection patch:**
   - Remove `ModelDbAllCharactersPatch` once deterministic character registration is live.
   - Add explicit verification gate (character select presence, card pool availability, relic pool availability).

4. **Retain compatibility safeguards during transition:**
   - Keep Merchant / SealedDeck / LargeCapsule guards enabled until replacement path is proven.
   - Keep visual fallback patches until dedicated assets are complete and tested.

5. **Metadata/dependency alignment:**
   - Keep `GunslingerMod.json` as dependency authority.
   - Keep `mod_manifest.json` packaging-only.
   - Clarify csproj/runtime dependency policy in docs and release checklist.

6. **Localization validation step:**
   - Add a startup or CI validation utility that confirms all key sets exist for ENG/KOR/JPN across:
     - `cards`, `powers`, `relics`, `characters`, `static_hover_tips`.

## Safe migration order

1. **First file to change:** `ModEntry.cs`.
   - Make it a thin delegate to framework bootstrap and stop treating it as registration logic host.

2. **Then add framework registration files (new files first):**
   - Add bootstrap + registration modules before removing any current patch-based behavior.

3. **Earliest patch removable:** `src/GunslingerMod/Patches/NRestSiteRoom_UpgradeDebugPatch.cs`.
   - It is diagnostic-only and not structural.

4. **Patch that must remain until replacement is verified:**
   - `src/GunslingerMod/Patches/SealedDeck_GunslingerCompatPatch.cs` (highest-risk crash-prevention path).
   - Also keep `CardFactory_CreateForMerchantPatch.cs` during transition to avoid shop-generation regressions.

5. **Remove registration reflection patch only after explicit verification:**
   - Delete `ModelDbAllCharactersPatch` in `ModEntry.cs` only when character appears in selection, rewards, events, merchant, and compendium without reflection/cache resets.

6. **Keep custom after BaseLib migration (intentionally custom):**
   - `src/GunslingerMod/Models/Combat/BulletResolver.cs` and ammo/cylinder semantics.
   - `src/GunslingerMod/Nodes/NGunslingerCylinderUi.cs` UX behavior.
   - `src/GunslingerMod/Models/RelicPools/GunslingerRelicPool.cs` starter-relic exclusion policy.
   - Character-specific compatibility guards where core assumptions still differ (e.g., LargeCapsule strike/defend fallback).

