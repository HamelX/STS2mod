# GUNSLINGER_BASELIB_MIGRATION_MASTERPLAN

## Executive summary

This repository already **declares** BaseLib in `GunslingerMod.json`, but the runtime architecture is still primarily a custom Harmony-driven integration that patches core getters and behaviors directly. The most critical migration task is to move from "patch-first registration" to a **BaseLib-first registration and lifecycle boundary** while preserving existing Gunslinger-specific combat logic (Cylinder/BulletResolver safety behavior).  

Recommended order:
1. Lock in BaseLib-first dependency + init specification.
2. Refactor registration/plumbing boundaries (character/cards/relics/localization/hooks).
3. Apply gameplay redesign only after migration scaffolding is stable.

---

## Current non-BaseLib architectural divergences

### 1) Initialization/registration is Harmony-reflection first, not BaseLib-first
- `ModEntry.Initialize()` calls `Harmony.PatchAll()` globally and uses a hard patch on `ModelDb.AllCharacters` to inject Gunslinger via reflection (`Type.GetType`, `MakeGenericMethod`, and private cache invalidation).  
- This is fragile to game updates and bypasses explicit framework-level registration ordering.

### 2) Character registration uses private cache resets
- `ModelDbAllCharactersPatch` clears `_allCharacterCardPools` / `_allCards` via reflection side effects.
- This is a strong sign registration lifecycle is currently "retroactive patching" instead of deterministic startup registration.

### 3) Dependency declaration is split and partially redundant
- `GunslingerMod.json` declares `BaseLib`, but build/runtime references in `GunslingerMod.csproj` are direct game DLL hint paths (`sts2.dll`, `GodotSharp.dll`, `0Harmony.dll`) from local install path.
- There is no single documented source of truth for dependency expectations and version compatibility.

### 4) Content boundaries are mixed
- Character/card/relic models are cleanly in `src/GunslingerMod/Models/*`, but framework integration logic sits in `ModEntry.cs` with broad patches, while gameplay-specific compatibility patches live adjacent to framework patches under `src/GunslingerMod/Patches/*`.
- This makes it easy for gameplay files to absorb framework concerns over time.

### 5) Compendium/pool compatibility relies on custom patches
- Merchant/SealedDeck/LargeCapsule behavior is stabilized using targeted Harmony patches (`CardFactory_CreateForMerchantPatch`, `SealedDeck_GunslingerCompatPatch`, `LargeCapsule_GunslingerCompatPatch`) rather than a centralized BaseLib-oriented compatibility adapter layer.

### 6) Manifest generation does not include dependency metadata parity
- Build target writes `mod_manifest.json` with only name/author/version/pck_name, while dependency is in `GunslingerMod.json`.
- Release packaging expectations are not explicitly codified (which manifest is authoritative, how dependency versions are validated).

---

## BaseLib integration plan

## A. Dependency + packaging plan (first)
1. Keep `GunslingerMod.json` as the authoritative mod dependency contract and explicitly document BaseLib minimum tested version.
2. Keep direct runtime DLL references for compile-time, but isolate them as **engine/runtime references**; document that BaseLib is runtime mod dependency, not raw DLL link.
3. Extend release checklist: package must include DLL + PCK + `GunslingerMod.json` with BaseLib dependency and tested version matrix.
4. Stop regenerating partial dependency metadata in `mod_manifest.json` as implicit source of truth; treat it as packaging metadata only.

## B. Initialization/registration migration
1. Replace `ModelDb.AllCharacters` postfix injection with a BaseLib-friendly registration entrypoint module (e.g., `Framework/Registration/GunslingerRegistration.cs`).
2. Move all registration calls into explicit staged boot sequence:
   - Stage 1: framework bootstrap (dependency checks/logging).
   - Stage 2: model registration (character/cardpool/relicpool/powers/dynamic vars).
   - Stage 3: hook/patch activation (compatibility patches only).
3. Keep Harmony only for missing engine hooks / known compatibility fixes (merchant/sealed deck/etc.), not primary content registration.

## C. Content organization boundary
Create two top-level boundaries:
- `src/GunslingerMod/Framework/*`:
  - dependency checks
  - registration orchestration
  - compatibility hook adapters
  - localization registration/index validation utilities
- `src/GunslingerMod/Gameplay/*` (can be alias/move from existing `Models/*` over time):
  - cards, powers, combat resolvers, character identity systems

## D. Patch policy
- **Keep custom patches** where they protect real engine-edge safety (SealedDeck candidate exhaustion, merchant empty rarity bucket, event assumptions about Strike/Defend).  
- **Replace custom patches** used solely for registration injection and cache invalidation with deterministic BaseLib registration pipeline.

## E. Coexistence expectations with other BaseLib mods
- No patch should assume Gunslinger is the only custom character.
- All patch prefixes must early-return for non-Gunslinger characters (already mostly true; enforce as standard).
- Avoid static mutable global state outside combat-local context.
- Document compatibility surface: card pool queries, relic pool behavior, mod hook interactions.

---

## Gunslinger redesign vision after migration

Gunslinger should be a **system character** with one clear center and two support layers:
- Core body/interface: **Cylinder**
- Primary engine language: **Imprint**
- Support engine A: **Tracer/Hunt tempo**
- Support engine B: **Seal slow finisher**
- Secondary rewards only: **Ricochet / Overclock**

Key outcome: remove the current "many equal archetype pillars" feel and make every package read as part of one combat grammar.

## Core identity
- Every turn should ask: *what is in chamber now, what am I building (Imprint), and which tempo/finisher line am I preparing?*
- Empty chamber turns should be recoverable (Reload/access cards), never class-locking.

## System layers
1. **Cylinder (body):** load/consume/rotate as universal input layer.
2. **Imprint (primary axis):** rewarded by successful fire and spent by high-value converts/payoffs.
3. **Tracer/Hunt (tempo):** helps sequencing, draw/flow and selective extra fire windows.
4. **Seal (slow payoff):** setup/position/convert line for controlled burst turns.
5. **Ricochet/Overclock (auxiliary):** side rewards that enhance core loop, not independent deck identity endpoints.

---

## Starter deck philosophy

Preferred target deck is accepted:  
- Shoot x4  
- Defend x4  
- Reload x1  
- Etched Tracer x1

Why this is preferred over current list (`Defend x3`, `Reload x2`):
- +1 Defend better teaches "defensive identity" and reduces dead-turn pressure.
- Reload x1 still teaches ammo flow without over-centralizing non-interactive recovery.
- Etched Tracer remains the support-axis onboarding card.

## Starter relic philosophy
- `CylinderRelic` must remain starter-only (already enforced in relic pool unlock filtering).
- Keep opening stability behavior (early-load assistance) but avoid making relic solve all ammo mistakes.
- Starter relic should bootstrap baseline loop, not replace reload decision-making.

---

## Card pool buckets (target structure)

- **Foundation (Basic/Common):** Shoot, DefendGunslinger, Reload, TakeCover, EtchedTracer, TracerStrike, SealLoad (limited), one-to-two neutral flow cards.
- **Primary engine (Common/Uncommon):** Imprint generation + controlled spenders.
- **Tempo support (Common/Uncommon):** Hunt start/Tracer conversion/flow manipulators.
- **Slow finisher support (Uncommon/Rare):** seal setup, positioning, release.
- **Auxiliary reward (Uncommon/Rare):** ricochet/overclock cards that reward core success but do not define standalone build rails.

---

## Keyword / tooltip / UI / compendium implications

1. Canonical keyword language must align across EN/KR/JP:
   - Cylinder terms (loaded/empty/current chamber)
   - Imprint gain/spend verbs
   - Hunt-start state naming
   - Seal release wording
2. Update static hover tips to match redesigned pillar hierarchy.
3. Ensure compendium visibility matches active pool (exclude generation-only / deprecated cards consistently).
4. Keep cylinder UI hooks but move registration into framework boundary and avoid repeated ad-hoc node lifecycle logic.

---

## Balance risks

- Over-rewarding Imprint can trivialize energy economy and compress card costs.
- Tracer tempo can accidentally become pure draw-engine if not capped by meaningful ammo constraints.
- Seal finishers can create binary fights if setup safety is too high.
- Demoting Ricochet/Overclock may over-nerf existing rare payoff excitement if not compensated by cleaner role definition.

## Regression risks

High-risk areas during migration:
- Character registration no longer populates card pools (if AllCharacters patch removed before replacement).
- Pool/reflection cache assumptions breaking compendium/merchant generation.
- Localization key drift during card merge/cut pass.
- UI lifecycle leaks for cylinder widget.

Required explicit regression checklist:
- [ ] No softlock when target dies mid multi-shot.
- [ ] No broken ammo flow in starter deck loops.
- [ ] Starter relic does not enter random relic rewards.
- [ ] Card pool registration remains valid for rewards/merchant/events.
- [ ] Localization keys resolve for all active cards/powers/relics.
- [ ] Compendium/library shows intended active set only.
- [ ] No obvious breakage introduced by BaseLib alignment changes.
- [ ] No obvious compatibility regression with other BaseLib-based mods.

---

## Complete card audit table

| current_card | current_role | problem | action (keep / rework / merge / cut) | new_role | implementation_notes |
|---|---|---|---|---|---|
| Shoot | core fire basic | fine baseline, keep safety behavior | keep | core trigger-pull basic | preserve dry-fire draw + rotation logic |
| DefendGunslinger | core defense basic | starter defense count too low currently | keep | core defense basic | use 4 copies in starter deck |
| Reload | ammo reset basic | starter has too much reload weight | rework | one-copy starter stabilizer | keep card mostly intact; adjust deck composition |
| TakeCover | defensive ammo-state payoff | overlaps with generic block if too flat | rework | defensive identity with empty-chamber scaling | keep as class-defensive signature |
| Evasion | defensive spike | can dilute cylinder gameplay if too generic | rework | situational defense tech | keep as non-core optional common/uncommon |
| EchoNote | imprint spend draw | may become generic draw staple | rework | imprint conversion utility | gate draw ceiling to avoid auto-pick |
| QuickRack | tracer/hunt bridge | previously overloaded identity | rework | tempo bridge | keep hunt conditional fire identity |
| HotChamber | tracer loader | can be over-efficient draw engine | rework | tracer tempo injector | cap draw reliability vs hunt state |
| Panning | multi-shot dump | high variance lethality pacing | keep | cylinder dump payoff | keep combat-end safety guards |
| SprayFire | random multi-shot AoE | overlaps with Panning in role | merge | alternative panning variant | merge with Panning or keep only one uncommon |
| SealLoad | seal entry | okay, but must remain slow setup | keep | seal setup anchor | maintain one-seal cap behavior |
| SealPressure | seal loader/defense | role overlap with SealLoad/Tension | merge | seal setup branch | fold into SealLoad or SealTension package |
| EtchedTracer | starter support axis intro | good onboarding card | keep | starter tempo intro | keep in starter deck |
| TracerBrand | tracer-mark bridge | overlaps with TracerStrike/ReadMark space | rework | tracer to imprint bridge | simplify payoff text |
| SealBurstLoad | burst seal setup | overlaps with SealPressure/Load | merge | high-roll seal prep | fold into single uncommon seal setup card |
| BrandedShot | marked payoff shot | naming/role overlaps with other brand cards | rework | imprint/tracer bridge attack | ensure unique payoff trigger |
| TracerStrike | clean tracer attack | strong identity teacher | keep | tracer tempo core common | keep as early common |
| ReadTheMark | bridge utility | historically shifted role repeatedly | rework | defensive imprint bridge | avoid ricochet overemphasis |
| RicochetFollowUp | ricochet conditional attack | ricochet as pillar risk | rework | aux reward follow-up | requires core fire success first |
| TracerLoad | tracer mass loader | may flood tempo axis | rework | tracer reload specialist | keep but tune rarity/volume |
| SteadyAim | ricochet/imprint setup | duplicates primer roles | merge | precision setup tool | merge with RicochetPrimer-like role |
| RicochetPrimer | ricochet setup | too standalone for target redesign | rework | auxiliary setup reward | tie to imprint or tracer precondition |
| ImprintSqueeze | fast imprint gain | potentially too free engine | keep | explicit imprint accelerator | maintain discard/tempo cost |
| ImprintCompression | imprint engine power | can snowball passive draw | rework | controlled imprint sustain | threshold tuning + once/turn constraints |
| FanTheBrand | ricochet+imprint grant | ricochet pillar inflation | rework | aux bridge card | reduce standalone ricochet identity |
| RicochetShot | iconic ricochet payoff | currently archetype-defining | rework | rare auxiliary finisher | keep splashy but non-foundational |
| BankShot | ricochet conditional shot | good as bridge if toned | keep | auxiliary payoff attack | enforce precondition clarity |
| BarrageCollapse | ricochet AoE payoff | independent pillar risk | rework | rare side reward | keep rare only; no common support push |
| ReboundNet | ricochet stack setup | standalone package density too high | cut | n/a | remove to reduce pillar competition |
| ImprintManifestRicochet | imprint->ricochet conversion | centralizes off-core axis | rework | imprint conversion rare | keep as optional conversion, not lane core |
| TracerConversion | ammo conversion | good tempo glue | keep | tracer tempo utility | keep as support axis card |
| BallisticCompiler | tracer first-shot reward | strong identity signal | keep | tracer tempo engine | maintain once-per-turn behavior |
| ChainBurst | hunt bonus multi-fire | overlaps HuntTrigger/WalkingFire | merge | hunt payoff attack | merge into single hunt payoff line |
| WalkingFire | repeated fire payoff | good tempo finisher | keep | tracer/hunt payoff | keep with tuned hit count |
| TracerStorm | tracer burst payoff | may overlap WalkingFire/Trigger | rework | rare tracer climax | ensure distinct from WalkingFire |
| BlankFire | hunt-start + fire | good bridge if simple | keep | hunt activator | keep low complexity |
| HuntPrep | hunt setup | strong teaching card | keep | hunt setup uncommon | keep draw modest |
| CrossfireRhythm | rhythm payoff | identity unclear naming/effect | rework | tracer rhythm payoff | align with tempo burst language |
| TagBurst | imprint scaling attack | can become generic scaling nuke | rework | imprint payoff attack | enforce cost/condition tradeoff |
| SealBreak | seal release bridge | good midline seal card | keep | seal bridge release | preserve direct-seal fire behavior |
| CasingCount | empty-chamber reward | niche but teaches cylinder state | keep | cylinder-state utility | keep as identity reinforcement |
| SealTension | seal defend/grow | overlaps other seal prep cards | keep | seal defense bridge | keep as defensive seal setup |
| HuntTrigger | hunt bonus fire | overlaps ChainBurst/WalkingFire | rework | hunt conditional burst | simplify and differentiate |
| EmptyTheMagazine | reset/draw pivot | can hard-lock draw interactions | rework | reset utility card | preserve anti-infinite safeguards |
| OverclockDrum | tracer-trigger extra pull | risks independent pillar | rework | rare auxiliary tempo reward | keep non-stacking strictness |
| OverclockCharge | energy-on-shot-cycle | can become dominant engine | rework | rare aux economy reward | keep expensive + innate tradeoff |
| ExecutionShot | heavy finisher | good capstone but generic | keep | single-target finisher | tie benefit to core fire loop |
| SealedRicochet | seal+ricochet hybrid | cross-pillar complexity bloat | cut | n/a | remove or fold into one rare hybrid |
| ImprintSeal | imprint->seal bridge | good cross-axis glue | rework | controlled bridge tool | keep but reduce complexity |
| RicochetBurst | ricochet burst payoff | pillar inflation | cut | n/a | remove redundant ricochet finisher |
| ImprintIgnition | global bullet damage power | clean primary-axis payoff | keep | imprint scaling power | keep as core rare/power |
| SealRite | seal engine power | good slow finisher anchor | keep | seal package core rare | keep generation-only SealShot tie-ins |
| GrandRite | seal growth+release | overlaps SealReleaseKai if too similar | keep | intermediate seal release | maintain mid-step finisher role |
| SealReleaseKai | seal final payoff | good identity finisher | keep | top-end seal finisher | keep retained/rare feel |
| FinalVolley | late burst fire payoff | may overlap execution/walking lines | rework | burst payoff rare | ensure unique condition window |
| DeadAngle | technical shot payoff | role clarity uncertain | rework | precision niche payoff | clarify axis association |
| EtchedResonance | etched/seal resonance payoff | niche overlap with seal finishers | merge | seal/imprint resonance finisher | fold into one rare finisher slot |

---

## Repository-specific migration checklist

### csproj / references
- [ ] Keep direct `sts2.dll`, `GodotSharp.dll`, `0Harmony.dll` references only as runtime engine compile references.
- [ ] Add explicit documentation section in repo for required local props and game path setup.
- [ ] Document BaseLib compatibility target version in release notes/checklist.

### dependency declaration
- [ ] Treat `GunslingerMod.json` as dependency authority.
- [ ] Keep `dependencies: ["BaseLib"]` and add minimum tested version notation in docs/changelog.
- [ ] Ensure packaging always includes `GunslingerMod.json` alongside DLL/PCK.

### package / release expectations
- [ ] Clarify roles of `mod_manifest.json` (engine package metadata) vs `GunslingerMod.json` (mod dependency metadata).
- [ ] Add release gate: fail release if dependency metadata missing/mismatched.

### init / registration flow
- [ ] Move registration out of `ModelDb.AllCharacters` reflection patch into framework registration module.
- [ ] Keep `ModEntry.Initialize()` minimal: bootstrap + staged register + compat patch activation.
- [ ] Remove private cache reset reflection once deterministic registration is in place.

### content registration
- [ ] Centralize registration ordering for character, card pool, relic pool, powers, dynamic vars.
- [ ] Keep gameplay classes free from framework bootstrapping code.
- [ ] Keep compat patches scoped and guarded to Gunslinger character.

### localization / compendium / UI consistency
- [ ] Verify EN/KR/JP card keys for all active cards after merge/cut pass.
- [ ] Remove inactive cards from compendium-visible pools.
- [ ] Validate static hover tips for Cylinder/Imprint/Tracer/Hunt/Seal terminology.
- [ ] Ensure cylinder UI lifecycle create/destroy paths are robust across combat transitions.

### compatibility expectations with BaseLib-based mods
- [ ] No global behavior changes for non-Gunslinger characters.
- [ ] Hook and patch guards must early-exit when player character is not Gunslinger.
- [ ] Avoid assumptions about pool sizes, rarity availability, or exclusive ownership of hooks.

---

## File-level phased implementation order

## Phase 1 = BaseLib migration audit/spec
- Produce/maintain this masterplan document.
- Confirm all current registration/dependency touchpoints in:
  - `ModEntry.cs`
  - `GunslingerMod.csproj`
  - `GunslingerMod.json`
  - `mod_manifest.json`

## Phase 2 = registration/init/content plumbing refactor
- Add new framework boundary files:
  - `src/GunslingerMod/Framework/Bootstrap/GunslingerBootstrap.cs`
  - `src/GunslingerMod/Framework/Registration/GunslingerRegistration.cs`
  - `src/GunslingerMod/Framework/Compatibility/GunslingerCompatPatches.cs`
- Slim `ModEntry.cs` to staged bootstrap call(s).
- Remove character injection patch once replacement verified.

## Phase 3 = redesign doc + full card audit
- Keep this masterplan as source-of-truth spec.
- Add/update `docs/` architecture + redesign docs with finalized pillar model.

## Phase 4 = starter deck + core systems realignment
- Update `src/GunslingerMod/Models/Characters/Gunslinger.cs` starter deck to x4/x4/x1/+EtchedTracer.
- Verify `CylinderRelic` behavior remains starter-only and still teaches opening flow.
- Keep `BulletResolver` safety behavior unchanged.

## Phase 5 = common/uncommon card pool cleanup
- Edit `src/GunslingerMod/Models/CardPools/GunslingerCardPool.cs` to reflect KEEP/REWORK/MERGE/CUT decisions for common/uncommon.
- Implement merges/cuts in `src/GunslingerMod/Models/Cards/*` with localization sync.

## Phase 6 = rare/payoff cleanup
- Re-scope rares to: imprint primary, seal finisher, auxiliary ricochet/overclock rewards.
- Remove duplicated rare endpoints and tune role clarity.

## Phase 7 = localization/tooltips/compendium/UI consistency
- Update:
  - `GunslingerMod/localization/eng/cards.json`
  - `GunslingerMod/localization/kor/cards.json`
  - `GunslingerMod/localization/jpn/cards.json`
  - `*/static_hover_tips.json`
- Validate compendium/library and tooltip language consistency.

## Phase 8 = balance pass + regression pass
- Run build and any existing validation workflows.
- Execute explicit manual regression checklist for fire safety, starter flow, relic pool filtering, and compatibility hooks.

### Existing build/validation workflows discovered
- Build command referenced in repo docs: `dotnet build GunslingerMod.csproj -c Release`.
- No dedicated automated test suite discovered in repository structure; therefore manual regression matrix is required.

