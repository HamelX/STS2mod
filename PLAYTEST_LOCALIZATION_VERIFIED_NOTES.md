Playtest localization verification patch

Scope
- Verified active 38-card localization keys exist for kor/eng/jpn.
- Repaired broken text for:
  ECHO_NOTE, SIGIL_GUARD, SEAL_RAMPAGE, SEAL_INSIGHT
- Added missing SEAL_BARRIER.title in all locales.
- Saved as UTF-8 without BOM to avoid loader/encoding issues.

This patch is intentionally small and safe:
- It only touches localization cards.json files.
- It does not change gameplay code.
