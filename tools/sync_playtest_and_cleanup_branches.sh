#!/usr/bin/env bash
set -euo pipefail

REMOTE="${REMOTE:-origin}"
TARGET_BRANCH="${TARGET_BRANCH:-playtest-stable}"
MODE="${1:-dry-run}" # dry-run | delete

if ! git remote get-url "$REMOTE" >/dev/null 2>&1; then
  echo "[WARN] Remote '$REMOTE' is not configured in this repo."
  echo "       Configure remote first, then rerun this script."
  exit 0
fi

git fetch "$REMOTE" --prune

if ! git show-ref --verify --quiet "refs/remotes/$REMOTE/$TARGET_BRANCH"; then
  echo "[ERROR] Target remote branch '$REMOTE/$TARGET_BRANCH' does not exist."
  exit 1
fi

echo "[INFO] Target branch: $REMOTE/$TARGET_BRANCH"
echo "[INFO] Mode: $MODE"
echo

mapfile -t CODEX_BRANCHES < <(git for-each-ref \
  --format='%(refname:short)' "refs/remotes/$REMOTE/codex/*" | sort)

if [[ ${#CODEX_BRANCHES[@]} -eq 0 ]]; then
  echo "[INFO] No remote codex/* branches found."
  exit 0
fi

echo "[INFO] Found codex branches:"
printf ' - %s\n' "${CODEX_BRANCHES[@]}"
echo

echo "[INFO] Branch status vs $REMOTE/$TARGET_BRANCH:"
for rb in "${CODEX_BRANCHES[@]}"; do
  behind=$(git rev-list --count "$rb..$REMOTE/$TARGET_BRANCH")
  ahead=$(git rev-list --count "$REMOTE/$TARGET_BRANCH..$rb")
  echo " - $rb  behind:$behind ahead:$ahead"
done
echo

MERGED=()
UNMERGED=()
for rb in "${CODEX_BRANCHES[@]}"; do
  if git merge-base --is-ancestor "$rb" "$REMOTE/$TARGET_BRANCH"; then
    MERGED+=("$rb")
  else
    UNMERGED+=("$rb")
  fi
done

if [[ ${#MERGED[@]} -gt 0 ]]; then
  echo "[INFO] Safe-to-delete branches (already contained in $REMOTE/$TARGET_BRANCH):"
  printf ' - %s\n' "${MERGED[@]}"
else
  echo "[INFO] No safe-to-delete branches found yet."
fi
echo

if [[ ${#UNMERGED[@]} -gt 0 ]]; then
  echo "[INFO] Not merged yet (needs PR merge/cherry-pick first):"
  printf ' - %s\n' "${UNMERGED[@]}"
fi
echo

if [[ "$MODE" != "delete" ]]; then
  echo "[DRY-RUN] No branches were deleted."
  echo "          Run with: ./tools/sync_playtest_and_cleanup_branches.sh delete"
  exit 0
fi

if [[ ${#MERGED[@]} -eq 0 ]]; then
  echo "[INFO] Nothing to delete."
  exit 0
fi

for rb in "${MERGED[@]}"; do
  short="${rb#"$REMOTE/"}"
  echo "[DELETE] git push $REMOTE --delete $short"
  git push "$REMOTE" --delete "$short"
done

echo
echo "[DONE] Cleanup complete."
