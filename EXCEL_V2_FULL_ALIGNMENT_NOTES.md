# Excel v2 Full Alignment Package

This package treats `CARD_POOL_KO_v2.xlsx` as the single source of truth.

Applied in this package:
- Active card pool order aligned to the Excel v2 38-card list
- Korean card titles/descriptions rewritten to match the Excel v2 baseline
- `CARD_POOL_KO_LATEST.csv` and `CARD_POOL_KO_LATEST.generated.csv` regenerated from Excel v2
- `SEAL_BARRIER` included in the active card pool
- Legacy cards such as `TagBurst`, `GrandRite`, and `CrossfireRhythm` remain in source for reference but are no longer part of the active pool

Important:
- This package aligns card list / metadata reference / Korean localization.
- Card logic in the existing C# classes was left in place unless already matching the Excel intent; build/test verification is still required locally.
