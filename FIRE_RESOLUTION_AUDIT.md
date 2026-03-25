# Fire Resolution Audit Fixes

This pass focuses on bug-prone trigger-pull / bullet-resolution paths.

## Main fixes
- Unified opponent-alive checks to `IsAlive` for bullet resolution and multi-shot loops.
- Added `BulletResolver.ResolveAliveTarget(...)` so trigger-pull cards can safely retarget when the original target dies.
- Patched direct-fire cards that used `cardPlay.Target` after target death.
- Patched multi-shot cards that previously continued based on `!IsDead` checks.
- Patched seal and ricochet-adjacent legacy cards as well, to reduce future regressions.

## Files touched
- `src/GunslingerMod/Models/Combat/BulletResolver.cs`
- `src/GunslingerMod/Models/Cards/Shoot.cs`
- `src/GunslingerMod/Models/Cards/QuickRack.cs`
- `src/GunslingerMod/Models/Cards/ChainBurst.cs`
- `src/GunslingerMod/Models/Cards/WalkingFire.cs`
- `src/GunslingerMod/Models/Cards/BlankFire.cs`
- `src/GunslingerMod/Models/Cards/EtchedTracer.cs`
- `src/GunslingerMod/Models/Cards/ExecutionShot.cs`
- `src/GunslingerMod/Models/Cards/Panning.cs`
- `src/GunslingerMod/Models/Cards/SprayFire.cs`
- `src/GunslingerMod/Models/Cards/SealReleaseKai.cs`
- `src/GunslingerMod/Models/Cards/SealResonance.cs`
- `src/GunslingerMod/Models/Cards/TagBurst.cs`
- `src/GunslingerMod/Models/Cards/PrecisionShot.cs`
- `src/GunslingerMod/Models/Cards/RicochetSeal.cs`

## Why this matters
The previous mix of `IsAlive` and `!IsDead` checks could let follow-up logic run against an opponent who was no longer meaningfully alive, especially in the same action chain after a lethal shot. That is the most likely cause of the “last enemy dies but combat does not immediately resolve correctly” bug on trigger-pull cards.
