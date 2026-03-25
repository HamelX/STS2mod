# Tracer Realignment Plan (minimal, actionable)

## Goal
Make **Tracer** read as "many low-damage shots" instead of "generic Imprint/Ricochet utility."

## 1) Tracer-adjacent card classification

### Keep in Tracer package (bullet-rain identity)
- **Tracer Load** (ammo injector)
- **Shoot** (core trigger-pull)
- **Precision Shot** (single-shot skill test)
- **Panning** (multi-shot cylinder dump)
- **Walking Fire** (multi-hit barrage payoff)
- **Crossfire Rhythm** (Tracer-fired conditional burst)

### Move to generic Imprint/Ricochet package
- **Imprint**
- **Ricochet Shot**
- **Manifest Ricochet**
- **Fan the Brand** (Ricochet apply + Imprint gain)
- **Read the Mark** (Manifest: Ricochet check utility)
- **Tag Burst** (pure Imprint scaling attack)
- **Execution Shot** (large Imprint grant)

### Rename/reframe candidates (future pass)
- **Crossfire Rhythm** -> consider **Double Tap Rhythm** or **Tracer Cadence** (communicate repeat fire, not setup utility)
- **Walking Fire** -> keep name, but keep text/mechanics explicitly about repeated hits (done in this pass)

## 2) Top-priority low-risk changes (implemented now)
1. **Tracer damage down** to reinforce "chip damage" role.
2. **Tracer Load count up** to reinforce "many shots" role.
3. **Tracer payoff cards stop paying with Imprint utility** (multi-hit follow-through instead).

## 3) Minimal patch set applied
- `BulletResolver`: Tracer base damage **6 -> 4**.
- `TracerLoad`: loads **2/3 -> 3/4** (base/upgraded).
- `BulletResolver`: removed Tracer's extra Imprint gain on hit; Tracer now primarily sets `TracerFiredThisTurn` rhythm flag.
- `CrossfireRhythm`: when rhythm is active, now repeats damage instead of granting Imprint+draw.
- `WalkingFire`: changed from Imprint-scaling repeats to fixed barrage (**3 hits**, upgraded **4 hits**).
- Updated EN/JP/KR localization text for changed card behavior + Tracer hover note.

## 4) Next small follow-up (not in this patch)
- Physically split card pools so Ricochet/Imprint cards appear as a separate utility package instead of looking like Tracer's main lane.
- Optionally retune `Tag Burst` / `Fan the Brand` rarity or discovery weights after split.
