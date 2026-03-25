#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

if rg -n '^(<<<<<<<|=======|>>>>>>>)' --glob '!.git' .; then
  echo
  echo "[FAIL] Merge conflict markers detected."
  exit 1
fi

echo "[OK] No merge conflict markers found."
