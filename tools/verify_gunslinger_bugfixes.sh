#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

check() {
  local pattern="$1"
  local file="$2"
  local label="$3"
  if rg -n --fixed-strings "$pattern" "$file" >/dev/null; then
    echo "[OK] $label"
  else
    echo "[FAIL] $label"
    echo "       missing pattern: $pattern"
    echo "       file: $file"
    exit 1
  fi
}

check "ModelDb.Card<Reload>()," "src/GunslingerMod/Models/Characters/Gunslinger.cs" "Starting deck includes Reload"
check "ModelDb.Card<SealRite>()" "src/GunslingerMod/Models/Characters/Gunslinger.cs" "Starting deck includes SealRite (playtest)"
check "if (!IsUpgraded)" "src/GunslingerMod/Models/Cards/TakeCover.cs" "TakeCover+ skips ReloadLock"
check "await CardPileCmd.Draw(choiceContext, 1, Owner);" "src/GunslingerMod/Models/Cards/QuickRack.cs" "QuickRack OFF-rhythm draw"
check "else" "src/GunslingerMod/Models/Cards/HotChamber.cs" "HotChamber OFF-rhythm branch exists"
check "var anyShotSucceeded = false;" "src/GunslingerMod/Models/Cards/WalkingFire.cs" "WalkingFire tracks any successful shot"
check "await CardPileCmd.Draw(choiceContext, 1, Owner);" "src/GunslingerMod/Models/Cards/Shoot.cs" "Shoot dry-fire draw"
check "cylinder.CountSealLoaded() == 0" "src/GunslingerMod/Models/Cards/SealRite.cs" "SealRite autoloads Seal if empty"
check "combatState.RoundNumber == 1 || cylinder.CountLoaded() == 0" "src/GunslingerMod/Relics/CylinderRelic.cs" "CylinderRelic start/empty autoload logic"
check "__result ??= GD.Load<CompressedTexture2D>(GunslingerCharacterSelectIconPath);" "src/GunslingerMod/Patches/CharacterModel_GunslingerSilentVisualFallbackPatch.cs" "Character icon fallback load"
check "TryConsumeCurrentWithSealSkip" "src/GunslingerMod/Models/Combat/BulletResolver.cs" "Seal-skip trigger helper added"
check "[Gunslinger] SealSkip:" "src/GunslingerMod/Models/Combat/BulletResolver.cs" "Seal-skip debug log prefix"
check "loaded == CylinderPower.MaxRounds" "src/GunslingerMod/Models/Combat/BulletResolver.cs" "Seal-skip disabled only on full-seal cylinder"
check "Any(c => c.IsAlive && c.CurrentHp > 0)" "src/GunslingerMod/Models/Combat/BulletResolver.cs" "Alive-opponent checks use HP gate"
check "if (shotsFired > 0 && HasAliveOpponents())" "src/GunslingerMod/Models/Cards/SprayFire.cs" "SprayFire skips post-kill imprint apply"

echo
echo "[DONE] Gunslinger bugfix signatures verified."
