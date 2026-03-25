# Cardpool Structure Pass — 2026-03-21

## Goals covered
- Strengthen lane visibility around:
  - basic ammo stability
  - rapid-fire identity
  - seal high-risk/high-reward
  - ricochet as bridge module
- Keep cylinder load/fire/rotate core unchanged.
- Keep 3-energy baseline pacing intact.

## Card-pool composition changes
- Removed `ReadTheMark` from the active Gunslinger card pool to reduce bridge-lane dilution.
- Reassigned `ExecutionShot` in pool ordering from basic lane grouping to bridge-module grouping (imprint/ricochet handoff role).

## Rarity distribution tuning
### Basic stability / rapid-fire visibility up
- `PrecisionShot`: **Uncommon -> Common**
- `CrossfireRhythm`: **Uncommon -> Common**

### Ricochet bridge visibility tightened
- `FanTheBrand`: **Common -> Uncommon**
- `TagBurst`: **Common -> Uncommon**

### Seal lane pushed toward high-risk/high-reward
- `SealRite`: **Uncommon -> Rare**
- `SealOverload`: **Uncommon -> Rare**

### Cross-lane glue accessibility
- `EmptyTheMagazine`: **Rare -> Uncommon**

## Focused regression checks
1. **Ricochet trigger path**
   - `RicochetPower.AfterDamageReceived` still gates on bullet/ricochet context and positive unblocked damage.
   - Single-target fallback bounce behavior remains intact.
2. **Lethal combat-end safety**
   - `Panning` still short-circuits on dead target / no alive opponents before post-shot follow-up.
3. **Evasion expiry timing**
   - `EvasionPower.AfterTurnEnd` still expires after opposing-side turn end (not immediately on owner turn end).
4. **Seal selection/reward flow**
   - `SealAmplify` still uses chamber selection (with timeout fallback) on base and all-seal amplification on upgrade.
   - `CylinderPower.TryConsumeCurrent` still honors `SealProtectionPower` non-consume behavior.

## Build
- `dotnet build GunslingerMod.csproj -c Release` succeeded with 0 warnings / 0 errors.

## Localization
- No card rules text changed in this pass, so KR/JP card text sync remained unchanged.
- KR-first + JP sync policy should be applied on next pass that edits player-facing text.
