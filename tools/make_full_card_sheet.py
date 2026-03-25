import csv
import re
from pathlib import Path

pool_src = Path('C:/Users/Hamel/.openclaw/workspace/GunslingerMod/CARD_POOL_KO_LATEST.generated.csv')
meta_src = Path('C:/Users/Hamel/.openclaw/workspace/GunslingerMod/CARD_LIST_FOR_EXCEL.csv')
out = Path('C:/Users/Hamel/.openclaw/workspace/GunslingerMod/CARD_POOL_KO_FULL.csv')

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


def parse_meta_tuple(raw: str):
    # e.g. "1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy"
    if not raw:
        return '', '', '', ''
    parts = [p.strip() for p in raw.split(',')]
    cost = parts[0] if len(parts) > 0 else ''
    ctype = parts[1].replace('CardType.', '') if len(parts) > 1 else ''
    rarity = parts[2].replace('CardRarity.', '') if len(parts) > 2 else ''
    target = parts[3].replace('TargetType.', '') if len(parts) > 3 else ''
    return cost, ctype, rarity, target


meta = {}
with meta_src.open('r', encoding='utf-8-sig', newline='') as f:
    r = csv.DictReader(f)
    for row in r:
        card_id = row.get('CardId', '').strip()
        if not card_id:
            continue
        meta[card_id] = row

# Fallback for cards that may be missing in CARD_LIST_FOR_EXCEL.csv
fallback_meta = {
    'DEFEND_GUNSLINGER': ('1', 'Skill', 'Basic', 'Self'),
    'IMPRINT_SQUEEZE': ('0', 'Skill', 'Uncommon', 'None'),
    'TRACER_CONVERSION': ('0', 'Skill', 'Uncommon', 'None'),
}

rows = []
with pool_src.open('r', encoding='utf-8-sig', newline='') as f:
    r = csv.DictReader(f)
    for row in r:
        card_id = row['CardId']
        m = meta.get(card_id, {})
        cost, ctype, rarity, target = parse_meta_tuple(m.get('코스트/타입/레어도(코드)', ''))
        if not (cost or ctype or rarity or target):
            cost, ctype, rarity, target = fallback_meta.get(card_id, ('', '', '', ''))

        raw = row['효과']
        base = clean(expand_ifupgraded(raw, use_upgraded=False))
        up = clean(expand_ifupgraded(raw, use_upgraded=True))

        rows.append({
            '번호': row['Order'],
            'CardId': card_id,
            '카드명': row['카드명'],
            '코스트': cost,
            '타입': ctype,
            '레어도': rarity,
            '타겟': target,
            '기본 효과': base,
            '강화 효과': up,
            '강화 차이 요약': '동일' if base == up else '강화 시 효과 변경',
        })

with out.open('w', encoding='utf-8-sig', newline='') as f:
    w = csv.DictWriter(
        f,
        fieldnames=['번호', 'CardId', '카드명', '코스트', '타입', '레어도', '타겟', '기본 효과', '강화 효과', '강화 차이 요약'],
    )
    w.writeheader()
    w.writerows(rows)

print(f'Wrote {out} ({len(rows)} rows)')
missing = [r['CardId'] for r in rows if not r['코스트'] and not r['타입'] and not r['레어도']]
if missing:
    print('Missing meta:', ', '.join(missing))
