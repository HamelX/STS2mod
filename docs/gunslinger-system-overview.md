# GunslingerMod System Overview

This document is meant to preserve the *actual current design* of the Gunslinger so future work can extend the kit without accidentally flattening it into a normal attack/block class.

> Pass-1 thematic canon terms are tracked in `docs/arcane-canon-pass1.md`.

## 1) Core class concept

The Gunslinger is built around a **6-chamber cylinder** that acts as both:

- a tactical ammo queue,
- a visible combat-state puzzle,
- and the class's main pacing lever.

The intended feel is **"revolver management first, card text second."** Cards are not supposed to be generic damage packets. Most of them either:

- load specific ammo into the cylinder,
- fire from the current chamber,
- manipulate chamber order,
- reward empties vs loaded rounds,
- or convert shots into secondary resources like **Imprint**.

### Design identity in one sentence

**You are playing the state of the cylinder.**

That means future cards should usually care about one or more of:

- current firing chamber,
- loaded vs empty chambers,
- ammo type,
- seal level,
- whether a shot actually fired,
- or whether the player has banked Imprint / Ricochet setup.

## 2) Current systems and how they interact

## 2.1 Cylinder state

Primary implementation: `Models/Powers/CylinderPower.cs`

The cylinder is represented as a hidden power with these main pieces:

- `MaxRounds = 6`
- `ChamberIndex` = current firing chamber
- one bitmask per ammo family:
  - `NormalMask`
  - `EnhancedMask`
  - `PenetratorMask`
  - `SealMask`
  - `TempSealMask`
- packed seal levels in `SealLevelsPacked` (1 byte per chamber)
- `Amount` is used as the visible loaded-count/UI sync value, not the full source of truth

### Important invariants

- A chamber can only hold **one ammo type**.
- `LoadedMask` is the union of all ammo masks.
- The power is **hidden**, but its `Amount` tracks loaded rounds for UI/card preview convenience.
- `ResetChambers()` clears all ammo and resets `ChamberIndex` to 0.
- When the cylinder becomes completely empty, `Reload` resets the chamber pointer before loading.

### Rotation semantics

- Chamber `0` is the **12 o'clock firing chamber**.
- Most attack cards follow the same pattern:
  1. try to consume current chamber,
  2. advance chamber,
  3. sync `CylinderPower.Amount`.
- `Shoot` explicitly treats play as a **trigger pull**: even if the current chamber is empty, the cylinder still rotates.

This is critical to class feel. Empty chambers are not just failure states; they are timing states.

## 2.2 Ammo types

Currently implemented ammo families:

- **Normal**: baseline shot damage
- **Enhanced**: flat stronger shot damage
- **Penetrator**: unblockable shot behavior hook exists, but the current card pool does not actively populate a penetrator package yet
- **Seal**: the most developed special ammo family
- **Temp Seal**: temporary seal bullets loaded beyond normal seal constraints and cleaned up at turn end

### Current shot damage baseline

Shot cards repeatedly use this current prototype damage model:

- Normal: `8`
- Enhanced: `10`
- Seal: `8 + sealLevel`
- Penetrator currently uses the normal damage baseline unless a card adds more

The same logic is reused by:

- `Shoot`
- `PrecisionShot`
- `ExecutionShot`
- `Panning`
- `SealReleaseKai`
- cylinder UI hover preview

That duplication means future rebalance work should ideally centralize the formula instead of editing many files by hand.

## 2.3 Seal system

Seal is the main alternate ammo package and currently the most mechanically complete archetype.

### Hard rules

- `MaxSealRounds = 2` for normal seal bullets
- seal levels are stored per chamber as bytes
- normal seal loading respects the cap
- `SealOverload` uses `TempSealMask` to bypass that cap temporarily
- end of turn:
  - temporary seal bullets are removed first
  - then `EnforceSealLimit()` trims seals back to the 2-round cap if needed

### End-of-turn trimming behavior

If seal count is over cap, the system keeps the **highest-level seals first**.
Tiebreaker is lower chamber index.
Everything after the top two gets cleared.

This is extremely important for future cards: if they create extra permanent seals, the extras may disappear at end of turn unless they also account for this rule.

### Seal thresholds

Current actual constants in code are:

- `SealThresholdExtraHit = 5`
- `SealThresholdUnblockable = 10`
- `SealThresholdOverpenetration = 15`

Effects:

- **5+**: extra hit on the same target
- **10+**: shot becomes unblockable
- **15+**: overpenetration/line damage behind the target

Note: some comments/UI strings still mention older 4/8/12 thresholds. The implementation is now **5/10/15** and docs/future text should treat that as canonical unless intentionally changed.

### Seal growth sources

Passive seal growth on `CylinderPower.AfterSideTurnStart` was explicitly removed.
Seal levels are now meant to come from:

- `SealLoad` overflow mode (`+1` to all loaded seals if already capped)
- `SealAmplify`
- `SealOpen+`
- `SealRitePower`
- `SealAssimilation` equalization
- any future Seal card that intentionally adds levels

That recent shift matters: the package is now **card-driven growth**, not free passive scaling.

## 2.4 Seal-specific cards by role

### Load / establish seals

- **SealLoad**
  - loads up to 2 Seal bullets, respecting global seal cap
  - if already at seal cap, converts into `+1` level to all loaded seals instead
  - upgrade grants `SealProtectionPower 1`

- **SealRite** / `SealRitePower`
  - start-of-turn engine
  - each turn: first grows all currently loaded seals by power amount, then tries to load that many real seal bullets (up to the normal permanent cap)
  - upgraded `SealRite` gives 2 stacks, so it both grows faster and loads more aggressively

- **SealOverload**
  - loads temporary Seal bullets into empty chambers
  - base = 1 temp seal, upgraded = 2
  - temp seals are removed at end of turn

### Find / position / expose seals

- **SealSearch**
  - draw 2/3, then moves the highest-level seal to the firing chamber
  - preserves whether that seal was permanent or temporary while moving it

- **SealOpen**
  - swaps the highest-level seal into the current firing chamber
  - preserves whether that seal was permanent or temporary while moving it
  - upgrade additionally gives that chamber `+2` seal level

These are the class's main "make the next trigger pull matter" tools.

### Grow / consolidate seals

- **SealAmplify**
  - base: pick one loaded seal and give it `+3`
  - upgrade: all loaded seals get `+4`
  - base version depends on a UI click event from the custom cylinder UI

- **SealAssimilation**
  - finds the highest-level seal (prefers current chamber on ties because current chamber seeds `bestLvl` first)
  - fills every empty chamber with **temporary** seal bullets at that level
  - then raises every loaded seal bullet up to that best level
  - exhausts
  - upgrade adds Retain

This is the most explosive "convert one great seal into a full board state" card. The overfill is intentionally temporary, so the fantasy turn exists without breaking the long-term 2-real-seal boundary.

### Payoff / conversion cards

- **SealResonance**
  - deals `2 x highest seal level`
  - does not consume seals
  - upgrade makes it free

- **SealBarrier**
  - gain block based on current seal count
  - useful because it rewards holding seals instead of cashing them immediately

- **SealCheck**
  - 0-cost conditional draw if any seals exist

- **SealInsight**
  - draw equal to seal count, or seal count + 1 upgraded

- **SealRampage**
  - gain energy equal to seal count
  - upgraded becomes free

- **SealProtection**
  - gives `SealProtectionPower` (1 or 2 upgraded)
  - the next protected seal shot(s) still fire, but the seal bullet is **not consumed**

- **SealReleaseKai**
  - fires all seal bullets by repeatedly swapping a seal into the firing chamber and pulling it
  - inherits normal seal thresholds (extra hit / unblockable / overpenetration)
  - upgraded becomes free

## 2.5 Imprint system

Primary implementation: `ImprintPower`

Imprint is currently a **simple counter resource**. It does not yet contain custom behavior by itself; the cards are what interpret it.

### Current generators

- `Shoot`: +1 per successful fired shot
- `PrecisionShot`: +2
- `ExecutionShot`: +3
- `Panning`: gain up to +3 based on shots fired

Important: shot cards only grant Imprint if a bullet actually fired. Empty trigger pulls rotate the cylinder but do not grant Imprint.

### Current spenders / payoff

- **RicochetShot**
  - costs 2 Imprint, or 1 upgraded
  - multi-enemy fight: applies `RicochetPower 1` to all living enemies
  - single-enemy fight: applies `RicochetPower 3` to the target

- **ImprintManifestRicochet**
  - costs 4 Imprint, or 3 upgraded
  - single enemy: applies `RicochetImprintPower 2`
  - multiple enemies: applies `RicochetImprintPower 1` to all

There is also an `ImprintDamageVar`, implying another imprint-scaling attack either exists elsewhere or is planned, but the currently active visible package is mostly a setup resource for ricochet patterns.

## 2.6 Ricochet system

Primary implementations:

- `BulletContext`
- `RicochetContext`
- `RicochetPower`
- `RicochetImprintPower`

### Why the context objects exist

Ricochet is implemented as a reactive damage hook on enemies. To avoid breaking card-source/UI assumptions, the mod uses async-local context flags instead of rewriting damage source identity.

- `BulletContext.Current` marks that a bullet is currently dealing damage and records ammo type + seal level.
- `RicochetContext.Current` marks that a ricochet bounce is currently being processed.

This means ricochet powers can distinguish:

- normal non-bullet damage -> ignore
- bullet damage -> trigger
- ricochet damage -> `RicochetPower` can continue chaining because it allows either bullet or ricochet context
- `RicochetImprintPower` only reacts to **bullet** damage, so it does **not** recurse off ricochet bounces

### Current ricochet math

When a marked enemy takes qualifying damage:

- bounce damage = `floor(received damage * 0.5)`
- a random other alive enemy is chosen
- one stack is consumed
- power auto-clears at next owner turn start

### Intent

- `RicochetPower` = broader ricochet package, including chain continuation
- `RicochetImprintPower` = more controlled imprint-only variant that avoids recursive bounce loops

## 2.7 Defensive / tempo systems

### Neutral block penalty

`CylinderRelic` ensures the player has `NeutralBlockPenaltyPower`, which multiplies block gained by `0.5`.

Interpretation: the class is intentionally **bad at generic block cards**. Survival is supposed to come more from cylinder-aware tools, timing, and pressure than from standard defend scaling.

### CylinderRelic

Starting relic behavior each player turn:

- on round 1, sets `CylinderPower` amount to 0
- ensures the block penalty exists
- creates a `Reload` in hand with `Exhaust` and `Ethereal`

This is a big part of the class skeleton. The class is balanced around receiving a free temporary reload each turn.

### TakeCover

- gains block from base block + empty chambers
- applies `ReloadLockPower`

### ReloadLockPower

- while active, the owner cannot play `Reload`
- it auto-clears at the start of the owner's next turn

Intent: if you cash in empties for defense, you do not immediately undo that state by reloading in the same turn.

### Evasion

Separate evasion power package exists (`EvasionPower`, `EvasionPlusPower`), functioning as another non-standard defensive line distinct from simple block stacking.

## 3) Card taxonomy and current design intent

The current pool naturally falls into these buckets.

### A. Basics / foundation

- `Shoot`
- `Reload`
- `DefendGunslinger`
- `Evasion`
- `Panning`

Intent:

- teach that ammo management matters immediately
- establish empties vs loaded rounds as meaningful
- let upgraded `Reload` represent stronger ammo quality, not bigger quantity
- make `Panning` the "cash out the whole cylinder" common

### B. Straight shot upgrades / gunline attacks

- `PrecisionShot`
- `ExecutionShot`

Intent:

- still respect current chamber ammo
- reward firing live rounds with stronger immediate effect and more Imprint generation
- remain legible extensions of `Shoot`, not separate damage systems

### C. Imprint -> Ricochet utility package

- `RicochetShot`
- `ImprintManifestRicochet`

Intent:

- turn prior successful shots into future spread damage
- encourage sequencing: first fire, then mark enemies, then keep firing

### D. Empty-cylinder / timing utility

- `TakeCover`
- `EmptyTheMagazine`

Intent:

- reward not treating the cylinder as a passive magazine
- create moments where discarding or preserving specific ammo types matters

`EmptyTheMagazine` is especially "taxonomy glue":

- Normal present -> draw
- Enhanced present -> energy
- Seal present -> apply Vulnerable to all enemies

So it converts ammo composition into utility instead of direct shot damage.

### E. Seal package

- setup: `SealLoad`, `SealRite`, `SealOverload`
- positioning: `SealSearch`, `SealOpen`
- growth: `SealAmplify`, `SealAssimilation`
- support/payoff: `SealCheck`, `SealInsight`, `SealBarrier`, `SealRampage`, `SealProtection`, `SealResonance`, `SealReleaseKai`

Intent:

- distinct sub-archetype based on a small number of premium bullets with escalating value
- strong reward for sequencing and chamber manipulation
- high ceiling, but deliberately capped by only keeping two real seals

## 4) Hidden rules and implementation constraints

These are the rules most likely to be forgotten and accidentally broken.

### 4.1 `CylinderPower.Amount` is not the full model

The true state is the bitmasks + chamber index + packed seal levels.
`Amount` is effectively a synced convenience count.

If a card edits the cylinder directly, it should usually follow up with:

- `await PowerCmd.SetAmount<CylinderPower>(..., cylinder.CountLoaded(), ...)`

Otherwise previews/UI can drift.

### 4.2 Rotation is part of gameplay, not just presentation

Do not treat `AdvanceChamber()` as optional polish.
The firing pointer is a core mechanical state.

### 4.3 Empty trigger pulls matter

`Shoot` rotates even on empty.
That means future "fire current chamber" cards need a deliberate answer to:

- should it rotate on whiff?
- should it grant resource on whiff?
- should it consume setup power on whiff?

Current class identity strongly prefers: **rotate on trigger pull, reward only on successful fire**.

### 4.4 Seal UI selection dependency

Base `SealAmplify` waits on `NGunslingerCylinderUi.ChamberClicked`.
That means:

- it depends on the custom cylinder UI existing,
- it depends on chamber hover/click nodes staying interactable,
- and any future chamber-selection cards can reuse this pattern, but they should handle cancellation/timeouts more gracefully if UX issues appear.

### 4.5 Tooltip/UI and code comments are partially stale

There are a few historical mismatches:

- comments mentioning old seal thresholds (4/8/12)
- hover-tip comments still referencing those numbers
- some Korean comments are mojibaked/garbled
- UI preview comments still call the shot damage values prototype constants

Future design work should trust current executable constants first, comments second.

### 4.6 Seal protection changes consumption, not firing

`TryConsumeCurrent()` on a protected seal returns success and preserves the bullet.
That means protected seal shots:

- still count as shots,
- still use current seal level,
- still trigger bullet-context hooks,
- but do not leave the chamber.

This is a major combo enabler and should be considered when creating new repeated-fire cards.

### 4.7 Temp seals are real shots for the turn

`TempSealMask` bullets:

- behave like seal bullets while present
- contribute to `CountSealLoaded()`
- can gain seal levels
- vanish at end of turn

So future cards that scale with seal count or level automatically interact with overload turns.

### 4.8 SealAssimilation intentionally overfills before cleanup

`SealAssimilation` can fill every empty chamber with seals, even though the long-term cap is 2.
Those extra seals are intentionally loaded as **temporary** seals, not real ones.
That is not a bug by itself; it creates a temporary explosive turn. End-of-turn cleanup is what restores the archetype boundary.

### 4.9 Ricochet recursion rules are asymmetric on purpose

- `RicochetPower` can trigger from bullet **or ricochet** context
- `RicochetImprintPower` only triggers from **bullet** context

Do not "clean this up" into symmetry unless you explicitly want to alter combo ceilings.

### 4.10 Overpenetration uses battlefield position heuristics

`OverpenetrationHelper` tries to determine enemies "behind" the target by creature-node X position, with encounter-order fallback.

Implications:

- this is positional approximation, not a formal lane system
- weird layouts may produce unexpected behind-target ordering
- changing combat-node lookup behavior can indirectly change seal overpenetration behavior

## 5) Known balance assumptions and recent changes

Recent git history strongly suggests the current direction:

- `93af1f6` Add ricochet, cover, and empty-magazine utility cards
- `36f6fc4` Fix Gunslinger ricochet hooks and Shoot preview/build
- `0b71d84` Stabilize Gunslinger card previews and panning damage
- `1a00178` Stabilize ricochet handling and wire manifest localization
- `891943d` Rebalance Seal package around card-driven growth

### Interpreting the current balance model

1. **Seal growth should be earned by cards**, not passively farmed.
2. **The class is compensated for weak generic block** by better tempo/pressure tools and cylinder-aware defense.
3. **Reload is ubiquitous** because the relic injects a temporary Reload every turn.
4. **Seal value is concentrated in a few bullets**, not in filling the entire cylinder permanently.
5. **Imprint is a secondary reward track for successful firing**, not a totally separate archetype yet.
6. **Panning is intentionally capped on Imprint gain** (`max +3`) so full-cylinder dumps are explosive but not infinite resource engines.
7. **Upgraded Reload changes quality, not quantity**: enhanced rounds instead of more rounds.

### Practical assumptions to preserve

- A normal live shot baseline around 8 damage is the center of the current math.
- Enhanced is a modest premium, not an entirely new shot class.
- Seal bullets are stronger because they scale by setup time and thresholds.
- The player should often care whether chambers are **empty**, not just whether ammo exists.
- The player should frequently choose between:
  - firing now,
  - setting up a better next chamber,
  - holding seals for payoff cards,
  - or spending empties/loaded ammo composition on utility.

## 6) Guidance for future additions

## 6.1 Good future card patterns

New cards will fit cleanly if they do one of these:

- **Manipulate chamber order**
  - swap current chamber with another
  - rotate by 1/2/3
  - inspect top/current/next chamber
- **Reward exact cylinder states**
  - empties
  - number of unique ammo types loaded
  - whether current chamber is loaded
  - whether next chamber is live
- **Convert successful shots into secondary payoff**
  - more Imprint uses
  - ammo-type-specific rider effects
- **Create short-lived burst states**
  - temporary ammo
  - one-turn chamber rules
  - preserve/freeze current chamber
- **Expand the existing Seal subgame** without violating the 2-real-seal identity

## 6.2 Patterns to avoid

Avoid cards that:

- ignore the cylinder entirely and are just efficient generic attacks/skills
- create passive, automatic seal scaling every turn with no card investment
- permanently fill the cylinder with premium ammo too easily
- produce large generic block without interacting with empties, seals, evasion, or timing
- spend Imprint in ways that make firing order irrelevant

If a card would also make sense on any normal class with only number tweaks, it is probably not Gunslinger enough.

## 6.3 Implementation checklist for new shot cards

If you add a new fire/current-chamber card, check all of these:

- Does it call `TryConsumeCurrent()`?
- Does it rotate with `AdvanceChamber()`? If not, is that intentional?
- Does it sync `CylinderPower.Amount` after modifying the cylinder?
- Does it set/clear `BulletContext.Current` around damage?
- Does it honor seal threshold behavior, or intentionally bypass it?
- Does it interact with `SealProtectionPower` correctly through `TryConsumeCurrent()`?
- Should it grant Imprint only on a successful shot?

## 6.4 Implementation checklist for new seal cards

- Respect `MaxSealRounds = 2` unless the card is explicitly a temporary over-cap effect
- Decide whether over-cap seals are:
  - temporary this turn, or
  - subject to end-of-turn pruning
- Remember temp seals are removed before seal-limit enforcement
- Consider whether the card wants the **highest-level seal**, **current seal**, or **all seals**
- Update `CylinderPower.Amount` if load count changes
- If a card requires player chamber selection, document its UI dependency

## 6.5 Implementation checklist for new ricochet / bullet-reactive effects

- Use `BulletContext` if the effect should only trigger from bullet damage
- Use `RicochetContext` carefully if chain/secondary damage should count
- Be explicit about whether recursive chaining is allowed
- Prefer the current async-local pattern over mutating card source identity

## 6.6 Where a future refactor would pay off most

If someone does a maintenance pass later, the highest-value cleanup targets are:

1. centralize bullet damage/threshold resolution in one helper
2. centralize seal-shot rider application (extra hit / unblockable / overpenetration)
3. clean stale comments/localization references to old threshold numbers
4. document or refactor chamber-selection UX for cards like `SealAmplify`
5. reduce repeated `SetAmount<CylinderPower>(CountLoaded())` boilerplate safely

## 7) Short summary to keep in your head

The Gunslinger is **not** a normal attack class with ammo flavor.

It is a **stateful cylinder class** where:

- `Reload` and free relic reloads create ammo,
- firing rotates the chamber and builds Imprint,
- empties are a usable resource,
- Seal is the premium setup package,
- Ricochet is the payoff for repeated bullet hits,
- and the class stays healthy only if cards keep interacting with the cylinder as a real board.
