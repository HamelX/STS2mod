# Excel v2 normalization pass

This package treats `CARD_POOL_KO_v2.xlsx` as the single source of truth.

Applied in this pass:
- Confirmed the active card pool is the Excel v2 38-card set.
- Regenerated `CARD_POOL_KO_LATEST.csv` and `CARD_POOL_KO_LATEST.generated.csv` from the Excel v2 sheet.
- Pruned `cards.json` in `kor`, `eng`, and `jpn` down to the active 38-card set only.
- Updated `BulletContext.IsBulletCardSource` to the current active bullet-firing cards only.
- Left legacy/inactive card classes in source for now to avoid risky compile-time deletions without a local build.

Remaining recommended follow-up:
- Build the project locally and fix any compile/runtime issues if they appear.
- Do a gameplay pass to verify that each active card's C# logic matches the Excel v2 text exactly.
- After build verification, archive or delete legacy/inactive card classes if desired.
