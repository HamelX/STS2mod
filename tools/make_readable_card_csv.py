import csv
import re
from pathlib import Path

src = Path('C:/Users/Hamel/.openclaw/workspace/GunslingerMod/CARD_POOL_KO_LATEST.generated.csv')
out = Path('C:/Users/Hamel/.openclaw/workspace/GunslingerMod/CARD_POOL_KO_READABLE.csv')

TAG_PREFIX = '{IfUpgraded:show:'


def extract_ifupgraded_block(text: str, start: int):
    i = start + len(TAG_PREFIX)
    depth = 1
    split_idx = -1

    while i < len(text):
        if text.startswith('{IfUpgraded:show:', i):
            depth += 1
            i += len(TAG_PREFIX)
            continue

        ch = text[i]
        if ch == '{':
            depth += 1
        elif ch == '}':
            depth -= 1
            if depth == 0:
                end = i
                break
        elif ch == '|' and depth == 1 and split_idx == -1:
            split_idx = i
        i += 1
    else:
        return None

    if split_idx == -1:
        return None

    up_part = text[start + len(TAG_PREFIX):split_idx]
    base_part = text[split_idx + 1:end]
    return up_part, base_part, end + 1


def expand_ifupgraded(text: str, use_upgraded: bool):
    out = []
    i = 0
    while i < len(text):
        idx = text.find(TAG_PREFIX, i)
        if idx == -1:
            out.append(text[i:])
            break

        out.append(text[i:idx])
        block = extract_ifupgraded_block(text, idx)
        if block is None:
            out.append(text[idx:])
            break

        up_part, base_part, next_i = block
        out.append(up_part if use_upgraded else base_part)
        i = next_i

    return ''.join(out)


def clean(text: str):
    text = text.replace('[gold]', '').replace('[/gold]', '')
    text = text.replace('\n', ' ').strip()
    text = re.sub(r"\s+", " ", text)
    return text


rows = []
with src.open('r', encoding='utf-8-sig', newline='') as f:
    reader = csv.DictReader(f)
    for row in reader:
        raw = row['효과']
        base = clean(expand_ifupgraded(raw, use_upgraded=False))
        up = clean(expand_ifupgraded(raw, use_upgraded=True))
        rows.append(
            {
                '번호': row['Order'],
                'CardId': row['CardId'],
                '카드명': row['카드명'],
                '기본 효과': base,
                '강화 효과': up,
                '강화 차이 요약': '동일' if base == up else '강화 시 효과 변경',
            }
        )

with out.open('w', encoding='utf-8-sig', newline='') as f:
    writer = csv.DictWriter(
        f,
        fieldnames=['번호', 'CardId', '카드명', '기본 효과', '강화 효과', '강화 차이 요약'],
    )
    writer.writeheader()
    writer.writerows(rows)

print(f'Wrote {out} ({len(rows)} rows)')
