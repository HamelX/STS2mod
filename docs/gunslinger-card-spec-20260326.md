# Gunslinger 전체 카드 사양 (현재 코드 기준)

- 기준 시점: 2026-03-28
- 소스: `src/GunslingerMod/Models/CardPools/GunslingerCardPool.cs` + 각 카드 클래스 + `GunslingerMod/localization/kor/cards.json`
- 시스템 메모: 실탄 발사 성공 시 공통 규칙으로 각인 +1을 획득합니다.
- 시스템 메모: 기본 실린더 파워에서는 턴 시작 자동 봉인 레벨 성장을 제공하지 않습니다.

| 클래스 | 키 | 코스트/타입/희귀도/타겟 | 카드명(ko) | 카드 설명(ko) |
|---|---|---|---|---|
| `Shoot` | `SHOOT` | `1, Attack, Basic, AnyEnemy` | 사격 | {IfUpgraded:show:격발 1회. 실탄이면 {Damage:diff()} 피해(+3)를 줍니다. 빈 약실이면 카드 1장을 뽑습니다.\|격발 1회. 실탄이면 {Damage:diff()} 피해를 줍니다. 빈 약실이면 카드 1장을 뽑습니다.} |
| `DefendGunslinger` | `DEFEND_GUNSLINGER` | `(확인 필요)` | 방어 | {Block:diff()} 방어도를 획득. |
| `Reload` | `RELOAD` | `0, Skill, Common, None` | 장전 | {IfUpgraded:show:일반탄 2발 장전(최대 6). 카드 1장 뽑기.\|일반탄 2발 장전(최대 6).} |
| `TakeCover` | `TAKE_COVER` | `1, Skill, Common, None` | 엄폐 | {IfUpgraded:show:{Block:diff()} 방어도 획득. 빈 약실 1개당 방어도 +1.\|{Block:diff()} 방어도 획득. 빈 약실 1개당 방어도 +1.} |
| `Evasion` | `EVASION` | `1, Skill, Common, None` | 회피 | {IfUpgraded:show:이번 턴 다음에 받는 피해를 절반으로 만듭니다. (코스트 0).\|이번 턴 다음에 받는 피해를 절반으로 만듭니다.} |
| `EchoNote` | `ECHO_NOTE` | `0, Skill, Common, None` | 에코 노트 | {IfUpgraded:show:각인 1소모. 카드 3장 뽑기.\|각인 1소모. 카드 2장 뽑기.} |
| `QuickRack` | `QUICK_RACK` | `1, Attack, Common, AnyEnemy` | 퀵 랙 | {IfUpgraded:show:추적탄 2발 장전. 사냥개시 상태면 격발 2회. 아니면 카드 1장 뽑기.\|추적탄 2발 장전. 사냥개시 상태면 격발 1회. 아니면 카드 1장 뽑기.} |
| `HotChamber` | `HOT_CHAMBER` | `0, Skill, Common, None` | 핫 챔버 | {IfUpgraded:show:추적탄 3발 장전. 사냥개시 ON이면 카드 2장 뽑기. 아니면 카드 1장 뽑기.\|추적탄 2발 장전. 사냥개시 ON이면 카드 2장 뽑기. 아니면 카드 1장 뽑기.} |
| `Panning` | `PANNING` | `3, Attack, Common, AnyEnemy` | 패닝 | {IfUpgraded:show:장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해. 보존.\|장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해.} |
| `SprayFire` | `SPRAY_FIRE` | `2, Attack, Common, None` | 난사 | 장전된 탄환을 무작위 적에게 모두 격발합니다. 총 {Damage:diff()} 피해를 줍니다. |
| `SealLoad` | `SEAL_LOAD` | `1, Skill, Common, None` | 봉인탄 장전 | {IfUpgraded:show:봉인탄 1발 장전(최대 1). 이미 장전되어 있으면 봉인 레벨 +1. 방어도 8 획득.\|봉인탄 1발 장전(최대 1). 이미 장전되어 있으면 봉인 레벨 +1. 방어도 5 획득.} |
| `EtchedTracer` | `ETCHED_TRACER` | `0, Attack, Common, AnyEnemy` | 에칭 트레이서 | {IfUpgraded:show:대상을 지정해 격발 1회. 그 후 추적탄을 2발 장전합니다.\|대상을 지정해 격발 1회. 그 후 추적탄을 1발 장전합니다.} |
| `TracerStrike` | `TRACER_STRIKE` | `1, Attack, Common, AnyEnemy` | 트레이서 스트라이크 | {IfUpgraded:show:추적탄 2발 장전. 격발 1회.\|추적탄 1발 장전. 격발 1회.} |
| `ReadTheMark` | `READ_THE_MARK` | `1, Skill, Common, None` | 리드 더 마크 | {IfUpgraded:show:{Block:diff()} 방어도 획득. 도탄이 있으면 카드 1장 뽑기. 각인 1 획득.\|{Block:diff()} 방어도 획득. 도탄이 있으면 카드 1장 뽑기. 각인 1 획득.} |
| `RicochetFollowUp` | `RICOCHET_FOLLOW_UP` | `1, Attack, Common, AnyEnemy` | 도탄 추격 | {IfUpgraded:show:격발 1회. 도탄이 있으면 +1회 격발.\|격발 1회. 도탄이 있으면 카드 1장 뽑기. 각인 1 획득.} |
| `ImprintSqueeze` | `IMPRINT_SQUEEZE` | `0, Skill, Uncommon, None` | 각인 압착 | {IfUpgraded:show:카드 1장 버림. 각인 3획득.\|카드 1장 버림. 각인 2획득.} |
| `ImprintCompression` | `IMPRINT_COMPRESSION` | `1, Power, Uncommon, None` | 각인 압축 | {IfUpgraded:show:턴 시작 시 각인이 2 이상이면 카드 1장 뽑기.\|턴 시작 시 각인이 3 이상이면 카드 1장 뽑기.} |
| `FanTheBrand` | `FAN_THE_BRAND` | `1, Skill, Common, None` | 낙인 흩뿌리기 | {IfUpgraded:show:도탄 4 획득. 각인 1 획득.\|도탄 3 획득. 각인 1 획득.} |
| `RicochetShot` | `RICOCHET_SHOT` | `2, Attack, Rare, AnyEnemy` | 도탄 사격 | {IfUpgraded:show:각인 3 소모. 격발 1회. 도탄 2 획득. 다음 공격 카드 비용 0. (코스트 1)\|각인 3 소모. 격발 1회. 도탄 2 획득. 다음 공격 카드 비용 0. (코스트 2)} |
| `BankShot` | `BANK_SHOT` | `1, Attack, Uncommon, AnyEnemy` | 뱅크 샷 | {IfUpgraded:show:격발 1회. 도탄이 있으면 도탄 1 소모. 추가로 피해 {Damage:diff()}.\|격발 1회. 도탄이 있으면 도탄 1 소모. 추가로 피해 {Damage:diff()}.} |
| `BarrageCollapse` | `BARRAGE_COLLAPSE` | `2, Skill, Rare, None` | 탄막 붕괴 | {IfUpgraded:show:도탄 5 획득. 이번 턴 처음 5번의 도탄 발동 시, 다른 모든 적에게 피해 3.\|도탄 4 획득. 이번 턴 처음 4번의 도탄 발동 시, 다른 모든 적에게 피해 3.} |
| `ReboundNet` | `REBOUND_NET` | `1, Skill, Uncommon, None` | 리바운드 넷 | {IfUpgraded:show:적 1명당 도탄 스택 1 획득. (코스트 0).\|적 1명당 도탄 스택 1 획득.} |
| `ImprintManifestRicochet` | `IMPRINT_MANIFEST_RICOCHET` | `2, Skill, Rare, None` | 현현: 도탄 | {IfUpgraded:show:각인 3소모. 이번 턴 모든 탄환 격발 시 도탄 발동.\|각인 4소모. 이번 턴 모든 탄환 격발 시 도탄 발동.} |
| `TracerConversion` | `TRACER_CONVERSION` | `0, Skill, Uncommon, None` | 추적탄 변환 | {IfUpgraded:show:일반탄 최대 2발을 추적탄으로 변환. 카드 1장 뽑기.\|일반탄 최대 2발을 추적탄으로 변환.} |
| `BallisticCompiler` | `BALLISTIC_COMPILER` | `1, Power, Uncommon, None` | 탄도 컴파일러 | {IfUpgraded:show:매 턴 첫 Tracer 발사 시 카드 1장을 뽑고 각인을 2 획득.\|매 턴 첫 Tracer 발사 시 카드 1장을 뽑고 각인을 1 획득.} |
| `ChainBurst` | `CHAIN_BURST` | `1, Attack, Uncommon, AnyEnemy` | 체인 버스트 | {IfUpgraded:show:각인 2소모. 격발 1회. 사냥개시 상태면 +2회.\|각인 2소모. 격발 1회. 사냥개시 상태면 +1회.} |
| `WalkingFire` | `WALKING_FIRE` | `1, Attack, Uncommon, AnyEnemy` | 워킹 파이어 | {IfUpgraded:show:각인 3을 소모합니다. 격발 3회. 이번 카드로 한 번이라도 실탄 발사에 성공하면 추적탄 1발을 장전합니다.\|각인 3을 소모합니다. 격발 2회. 이번 카드로 한 번이라도 실탄 발사에 성공하면 추적탄 1발을 장전합니다.} |
| `BlankFire` | `BLANK_FIRE` | `0, Skill, Uncommon, None` | 블랭크 파이어 | {IfUpgraded:show:사냥개시 상태가 됩니다. 격발 1회. 카드 1장 뽑기.\|사냥개시 상태가 됩니다. 격발 1회.} |
| `HuntPrep` | `HUNT_PREP` | `1, Skill, Uncommon, None` | 사냥개시 준비 | {IfUpgraded:show:사냥개시 상태가 됩니다. 추적탄 2발 장전. 카드 2장 뽑기.\|사냥개시 상태가 됩니다. 추적탄 2발 장전. 카드 1장 뽑기.} |
| `SealBreak` | `SEAL_BREAK` | `1, Attack, Uncommon, AnyEnemy` | 봉인 파열 | {IfUpgraded:show:가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 장전된 봉인탄 1발을 직접 격발합니다. 발사 성공 시 카드 2장 뽑기.\|가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 장전된 봉인탄 1발을 직접 격발합니다. 발사 성공 시 카드 1장 뽑기.} |
| `SealTension` | `SEAL_TENSION` | `1, Skill, Uncommon, None` | 봉인 긴장 | {IfUpgraded:show:봉인탄이 장전되어 있으면: 방어도 10 획득. 해당 봉인 레벨 3 상승. 없으면 봉인탄 1발 장전.\|봉인탄이 장전되어 있으면: 방어도 7 획득. 해당 봉인 레벨 2 상승. 없으면 봉인탄 1발 장전.} |
| `EmptyTheMagazine` | `EMPTY_THE_MAGAZINE` | `1, Skill, Uncommon, None` | 탄창 비우기 | 장전된 탄환을 모두 제거합니다. 제거한 수만큼 카드를 뽑습니다. 그 후, 이번 턴에는 더 이상 카드를 뽑을 수 없습니다. |
| `OverclockDrum` | `OVERCLOCK_DRUM` | `2, Power, Rare, None` | 오버클록 드럼 | 매 턴 첫 Tracer 발사 시 방아쇠 1회 추가로 당깁니다. 이 효과는 중첩되지 않습니다. |
| `OverclockCharge` | `OVERCLOCK_CHARGE` | `3, Power, Rare, None` | 오버클록 장약 | {IfUpgraded:show:한 턴에 3번째 발사 성공 시 에너지 1 획득. 각인 1 획득. 선천성.\|한 턴에 3번째 발사 성공 시 에너지 1 획득. 각인 1 획득.} |
| `ExecutionShot` | `EXECUTION_SHOT` | `2, Attack, Rare, AnyEnemy` | 처형 사격 | {IfUpgraded:show:격발 1회. 대상 체력이 절반 이하면 피해 3배.\|격발 1회. 대상 체력이 절반 이하면 피해 2배.} |
| `ImprintIgnition` | `IMPRINT_IGNITION` | `2, Power, Rare, None` | 각인 점화 | {IfUpgraded:show:파워: 내 탄환 격발 피해 +2.\|파워: 내 탄환 격발 피해 +1.} |
| `SealRite` | `SEAL_RITE` | `1, Power, Rare, None` | 봉인 장약 | {IfUpgraded:show:파워: 턴 시작 시 봉인탄이 장전되어 있으면 장전된 봉인탄 레벨을 1 올리고 다음 봉인 사격 피해 +3을 저장합니다. 사용 시 봉인탄이 없다면 봉인탄 1발을 장전하고 임시 카드 「봉인 사격」 1장을 손에 추가합니다. 선천성.\|파워: 턴 시작 시 봉인탄이 장전되어 있으면 장전된 봉인탄 레벨을 1 올리고 다음 봉인 사격 피해 +3을 저장합니다. 사용 시 봉인탄이 없다면 봉인탄 1발을 장전하고 임시 카드 「봉인 사격」 1장을 손에 추가합니다.} |
| `GrandRite` | `GRAND_RITE` | `2, Skill, Rare, AnyEnemy` | 그랜드 라이트 | {IfUpgraded:show:가장 높은 레벨의 장전된 봉인탄 레벨을 3 올립니다. 그 봉인탄을 발사 칸으로 정렬합니다. 직접 격발 1회.\|가장 높은 레벨의 장전된 봉인탄 레벨을 2 올립니다. 그 봉인탄을 발사 칸으로 정렬합니다. 직접 격발 1회.} |
| `SealReleaseKai` | `SEAL_RELEASE_KAI` | `3, Attack, Rare, AnyEnemy` | 봉인 해방: 개 | {IfUpgraded:show:가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 2회 격발. 보존.\|가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 2회 격발.} |

## 2026-03-27 확장 패스 반영 사항

- 카드 풀에 13장 확장 반영(2026-03-27 초기 기준).
  - 재도입: `TagBurst`, `RicochetSeal`, `ReadTheMark`, `TracerLoad`, `CrossfireRhythm`
  - 신규: `SteadyAim`, `CasingCount`, `SealTension`, `HuntReload`, `FinalVolley`, `SealImprint`, `GrandChamber`, `DeadAngle` *(이후 Seal 축 정리에서 `GrandChamber`는 카드 풀 제외)*
- `ReadTheMark`은 회전 효과를 제거하고, 방어 + 도탄 상태 체크 드로우로 단순화.
- 확장 규칙에 맞춰 순수 회전/스왑 신규 카드는 추가하지 않음.
- 상태 보상 강화:
  - 빈 약실 상태 보상: `CasingCount`
  - 사냥개시(Tracer rhythm) 보상: `HuntReload`, `CrossfireRhythm`
  - Seal 준비/정렬/해방 보조: `SealTension`, `SealImprint`, `GrandChamber` *(초기안, 최신안에서는 `GrandChamber` 제외)*
  - Imprint↔Ricochet 연결 강화: `TagBurst`, `SteadyAim`, `DeadAngle`, `RicochetSeal`

### 이번 패스의 의도적 구현 메모

- `GrandChamber`의 "한 턴 보너스"는 별도 신규 상태를 만들지 않고, 정렬된 Seal 레벨 +1로 처리했습니다. *(해당 카드는 이후 카드 풀 제외)*
- `FinalVolley`의 에너지 환급 조건은 "처음 2회 격발이 모두 실탄 발사 성공"으로 계산합니다(업그레이드 시 3회 격발 추가).

## 2026-03-27 정리 + 초반 파워플로어 보정

- 시작 덱을 `Shoot x4 / Defend x3 / Reload x2 / EtchedTracer x1`로 조정.
- `CylinderRelic`은 전투 1라운드 시작 시 일반탄을 2발 자동 장전(이후에는 완전 빈 실린더일 때만 1발 자동 장전).
- `TagBurst` 설명을 수치형 문구로 명확화:
  - `TagBurst` 설명(기본): 격발 1회, 발사 성공 시 추가 {Damage:diff()} 피해 + 각인당 피해 1, 도탄 2 획득.
  - `TagBurst` 설명(업그레이드): 격발 1회, 발사 성공 시 추가 {Damage:diff()} 피해 + 각인당 피해 2, 도탄 3 획득.
- `OverclockDrum`은 효과를 비중첩(single)로 고정해 오해 소지가 있는 가짜 중첩을 제거.
- `SteadyAim` 상향: 기본 도탄 2, 업그레이드 도탄 3.
- `FanTheBrand` 상향: 기본 도탄 3 + 각인 1, 업그레이드 도탄 4 + 각인 1.
- `FanTheBrand` 희귀도 조정: Uncommon → Common.


## 2026-03-27 Seal 축 정리 패스

- `SealOpen` 제거: 카드 풀/활성 설계에서 삭제.
- `GrandChamber` 제거: 카드 풀에서 삭제(중복 정렬 역할 축소).
- `GrandRite` 재설계: 단일 봉인탄 성장(+2/+3) + 내부 정렬 + 즉시 1회 해방.
- `SealReleaseKai` 정렬 내장: 최고 레벨 봉인탄 정렬 후 2회 발사.
- `RicochetSeal` 재설계: 도탄 보상 제거, 봉인탄 직접 격발 성공 시 드로우 보상(1/2).

## 2026-03-27 Gunslinger 정리/리밸런스 (이번 변경)

- Seal 축을 **소형 서브테마(6장 패키지)**로 축소:
  - 유지: `SealLoad`, `SealTension`, `SealRite`, `RicochetSeal`, `GrandRite`, `SealReleaseKai`
  - 카드 풀 제외: `SealSearch`, `SigilGuard`, `SealRampage`, `SealAmplify`, `SealImprint`, `SealInsight`, `SealBarrier`, `GrandChamber`, `SealOpen`
- `ImprintIgnition` 효과 변경: "각인 소비 카드 피해 증가"가 아니라 "모든 탄환 격발 피해 증가(+1/+2)"로 통합.
- Seal 해방 흐름 정리:
  - 중간 해방: `GrandRite` (성장 + 내부 정렬 + 즉시 1회 격발)
  - 최종 해방: `SealReleaseKai` (내부 정렬 + 2회 격발)
  - 브리지 해방: `RicochetSeal` (내부 정렬 + 1회 직접 격발 + 성공 시 드로우)

## 2026-03-27 Ricochet/Tracer 메인축 보강 패스

- 신규 카드 5장 추가(메인축 밀도 보강):
  - Ricochet/Imprint: `RicochetPrimer`(공용 진입), `BankShot`(전투형 브리지), `BarrageCollapse`(광역 페이오프)
  - Tracer/Hunt-start: `TracerStrike`(즉시 전투형 공용), `HuntTrigger`(사냥개시 보상형 언커먼)
- `HotChamber` 조정: 코스트 `1 → 0`, 각인 요구 제거, 업그레이드 시 장전량 `2 → 3`으로 단순 상향.
- `ReadTheMark` 블록 수치 조정: `7 → 8`(업그레이드 시 +3 유지).
- 봉인(Seal) 축 카드 추가/확장 없음(서브테마 유지).

### 이번 패스 텍스트/동작 동기화 상세

- `READ_THE_MARK`
  - 설명: `{IfUpgraded:show:{Block:diff()} 방어도를 획득. 도탄이 있으면 카드 1장 뽑기. 각인 1 획득.|{Block:diff()} 방어도를 획득. 도탄이 있으면 카드 1장 뽑기. 각인 1 획득.}`
  - 동작: 방어도 획득 후, `RicochetPower` 또는 `RicochetImprintPower`가 1 이상이면 카드 1장을 뽑고 각인 1을 획득.
- `HOT_CHAMBER`
  - 설명: `{IfUpgraded:show:추적탄 3발 장전. 사냥개시 ON이면 카드 2장 뽑기. 아니면 카드 1장 뽑기.|추적탄 2발 장전. 사냥개시 ON이면 카드 2장 뽑기. 아니면 카드 1장 뽑기.}`
  - 동작: 코스트 0. 각인 소모 없음. 장전 후 사냥개시 상태 여부에 따라 2/1 드로우.
- `RICOCHET_PRIMER`
  - 설명: `{IfUpgraded:show:도탄 3 획득. 각인 1 획득.|도탄 2 획득. 각인 1 획득.}`
  - 동작: 조건 없이 도탄 + 각인 즉시 획득.
- `BANK_SHOT`
  - 설명: `{IfUpgraded:show:격발 1회. 도탄이 있으면 도탄 1 소모. 추가로 피해 {Damage:diff()}.|격발 1회. 도탄이 있으면 도탄 1 소모. 추가로 피해 {Damage:diff()}.}`
  - 동작: 격발 1회 수행 후, 도탄(`RicochetPower` 또는 `RicochetImprintPower`)이 있으면 1 소모하고 추가 피해를 줍니다.
- `TRACER_STRIKE`
  - 설명: `{IfUpgraded:show:추적탄 2발 장전. 격발 1회.|추적탄 1발 장전. 격발 1회.}`
  - 동작: 먼저 추적탄 장전 후 격발 1회 수행.
- `HUNT_TRIGGER`
  - 설명: `{IfUpgraded:show:격발 1회. 사냥개시 ON이면 +2회 격발. 아니면 추적탄 1발 장전.|격발 1회. 사냥개시 ON이면 +1회 격발. 아니면 추적탄 1발 장전.}`
  - 동작: 기본 격발 1회 후, 사냥개시 ON이면 추가 격발. OFF면 추적탄 1발 장전.
- `BARRAGE_COLLAPSE`
  - 설명: `{IfUpgraded:show:도탄 5 획득. 이번 턴 처음 5번의 도탄 발동 시, 다른 모든 적에게 피해 3.|도탄 4 획득. 이번 턴 처음 4번의 도탄 발동 시, 다른 모든 적에게 피해 3.}`
  - 동작: `BarrageCollapsePower`로 이번 턴 처음 N번의 도탄 발동마다 다른 모든 적에게 피해 3을 추가합니다.


## 2026-03-28 Gunslinger 풀 오버홀 패스

- 메인 축 고정:
  - Tracer/Hunt-start
  - Imprint/Ricochet
- Seal 축은 소형 6장 서브 패키지로 유지:
  - `SealLoad`, `SealTension`, `SealRite`, `SealBreak`, `GrandRite`, `SealReleaseKai`
- 필수 리워크 반영:
  - `TakeCover`: 재장전 봉쇄 제거, 빈 약실 보정 블록형으로 단순화.
  - `QuickRack`: 각인 소모/플레이 게이트 제거, Hunt-start 상태일 때 격발 보상을 받도록 재설계.
  - `BlankFire`: 각인 소모 제거, Hunt-start 상태 부여 + 격발 1회로 단순화(강화 시 1드로우).
  - `TagBurst`: 발사 성공 시 추가타 수치 유지 + 도탄 획득량을 2/3으로 상향.
  - `RicochetShot`: 코스트 2(강화 1), 타입 Attack으로 전투 템포 개선.
  - `RicochetSeal` → `SealBreak`로 개명 및 역할 고정(Seal 해방 브리지).
- 신규 카드 2장 추가:
  - `RicochetFollowUp`(Common Attack): 도탄 보유 시 기본은 1드로우 + 각인 1, 강화 시 +1회 격발.
  - `HuntPrep`(Uncommon Skill): Hunt-start 상태 부여 + 추적탄 2발 장전 + 1/2드로우.
- 활성 풀 제외:
  - `HuntReload`
  - (`SealAmplify`, `SealImprint`, `SealInsight`, `SealBarrier`, `SigilGuard`, `SealSearch`, `SealRampage`, `GrandChamber`, `SealOpen`는 기존과 동일하게 비활성 유지)
- 코드/텍스트 동기화:
  - EN/JP/KR 카드 설명을 실제 수치/조건과 일치하도록 갱신.
  - 업그레이드 설명은 리워크/신규 카드 전부 `{IfUpgraded:show:UPG|BASE}` 전면 교체형으로 통일.

## 2026-03-29 Seal 보조 생성 패스

- 신규 생성 카드 `SealShot`(`SEAL_SHOT`) 추가:
  - 사양: `1, Attack, (생성 전용), AnyEnemy, Exhaust`
  - 효과: 가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬하고, 장전된 봉인탄 1발을 직접 격발.
  - 생성 전용 카드이므로 일반 카드 풀(`GunslingerCardPool`)에는 넣지 않음.
- `SealShot` 생성 조건(핵심 규칙):
  - “새 봉인탄 장전 성공” 시에만 손패에 임시 1장 생성.
  - 이미 봉인탄이 있어 레벨만 상승한 경우에는 생성하지 않음.
- 생성 연동 카드:
  - `SealLoad`: 실제 새 봉인탄 장전 성공 시 생성.
  - `SealPressure`: 장전 시도 중 새 봉인탄이 1회 이상 실제로 들어갔을 때만 생성.
  - `SealBurstLoad`: 장전 시도 중 새 봉인탄이 1회 이상 실제로 들어갔을 때만 생성.
  - `SealTension`: “봉인탄이 없어서 새로 장전한 분기”에서만 생성.
  - `SealRite`: 사용 시 봉인탄이 없어 새로 1발 장전한 경우에만 생성.
