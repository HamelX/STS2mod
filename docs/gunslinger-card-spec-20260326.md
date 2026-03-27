# Gunslinger 전체 카드 사양 (현재 코드 기준)

- 기준 시점: 2026-03-26
- 소스: `src/GunslingerMod/Models/CardPools/GunslingerCardPool.cs` + 각 카드 클래스 + `GunslingerMod/localization/kor/cards.json`
- 시스템 메모: 실탄 발사 성공 시 공통 규칙으로 각인 +1을 획득합니다.
- 시스템 메모: 기본 실린더 파워에서는 턴 시작 자동 봉인 레벨 성장을 제공하지 않습니다.

| 클래스 | 키 | 코스트/타입/희귀도/타겟 | 카드명(ko) | 카드 설명(ko) |
|---|---|---|---|---|
| `Shoot` | `SHOOT` | `1, Attack, Basic, AnyEnemy` | 사격 | {IfUpgraded:show:격발 1회. 실탄이면 {Damage:diff()} 피해(+3)를 줍니다. 빈 약실이면 카드 1장을 뽑습니다.\|격발 1회. 실탄이면 {Damage:diff()} 피해를 줍니다. 빈 약실이면 카드 1장을 뽑습니다.} |
| `DefendGunslinger` | `DEFEND_GUNSLINGER` | `(확인 필요)` | 방어 | {Block:diff()} 방어도를 획득. |
| `Reload` | `RELOAD` | `0, Skill, Common, None` | 장전 | {IfUpgraded:show:일반탄 2발 장전(최대 6). 카드 1장 뽑기.\|일반탄 2발 장전(최대 6).} |
| `TakeCover` | `TAKE_COVER` | `1, Skill, Common, None` | 엄폐 | {IfUpgraded:show:{Block:diff()} 방어도를 얻습니다. 빈 약실 하나당 방어도를 +1 얻습니다.\|{Block:diff()} 방어도를 얻습니다. 빈 약실 하나당 방어도를 +1 얻습니다. 이번 턴 [재장전 불가] 상태가 됩니다.} |
| `Evasion` | `EVASION` | `1, Skill, Common, None` | 회피 | {IfUpgraded:show:이번 턴 다음에 받는 피해를 절반으로 만듭니다. (코스트 0).\|이번 턴 다음에 받는 피해를 절반으로 만듭니다.} |
| `EchoNote` | `ECHO_NOTE` | `0, Skill, Common, None` | 에코 노트 | {IfUpgraded:show:각인 1소모. 카드 3장 뽑기.\|각인 1소모. 카드 2장 뽑기.} |
| `QuickRack` | `QUICK_RACK` | `1, Attack, Common, AnyEnemy` | 퀵 랙 | {IfUpgraded:show:각인 1을 소모합니다. 추적탄 2발을 장전합니다. 사냥개시 상태면 격발 2회, 아니면 카드 1장 뽑기.\|각인 1을 소모합니다. 추적탄 2발을 장전합니다. 사냥개시 상태면 격발 1회, 아니면 카드 1장 뽑기.} |
| `HotChamber` | `HOT_CHAMBER` | `1, Skill, Common, None` | 핫 챔버 | {IfUpgraded:show:추적탄 2발을 장전합니다. 사냥개시 상태면 카드 2장을 뽑고, 아니면 카드 1장을 뽑습니다.\|추적탄 2발을 장전합니다. 사냥개시 상태면 카드 2장을 뽑고, 아니면 카드 1장을 뽑습니다. 기본: 각인 1 소모.} |
| `Panning` | `PANNING` | `3, Attack, Common, AnyEnemy` | 패닝 | {IfUpgraded:show:장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해. 보존.\|장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해.} |
| `SprayFire` | `SPRAY_FIRE` | `2, Attack, Common, None` | 난사 | 장전된 탄환을 무작위 적에게 모두 격발합니다. 총 {Damage:diff()} 피해를 줍니다. |
| `SealLoad` | `SEAL_LOAD` | `1, Skill, Common, None` | 봉인탄 장전 | {IfUpgraded:show:봉인탄 1발 장전(최대 1). 이미 장전되어 있으면 봉인 레벨 +1. 방어도 8 획득.\|봉인탄 1발 장전(최대 1). 이미 장전되어 있으면 봉인 레벨 +1. 방어도 5 획득.} |
| `SigilGuard` | `SIGIL_GUARD` | `1, Skill, Common, Self` | 시질 가드 | {IfUpgraded:show:방어도 12 획득. 봉인 레벨당 방어도 +1.\|방어도 8 획득. 봉인 레벨당 방어도 +1.} |
| `EtchedTracer` | `ETCHED_TRACER` | `0, Attack, Common, AnyEnemy` | 에칭 트레이서 | {IfUpgraded:show:대상을 지정해 격발 1회. 그 후 추적탄을 2발 장전합니다.\|대상을 지정해 격발 1회. 그 후 추적탄을 1발 장전합니다.} |
| `ImprintSqueeze` | `IMPRINT_SQUEEZE` | `0, Skill, Uncommon, None` | 각인 압착 | {IfUpgraded:show:카드 1장 버림. 각인 3획득.\|카드 1장 버림. 각인 2획득.} |
| `ImprintCompression` | `IMPRINT_COMPRESSION` | `1, Power, Uncommon, None` | 각인 압축 | {IfUpgraded:show:턴 시작 시 각인이 3 이상이면 카드 1장 뽑기.\|턴 시작 시 각인이 4 이상이면 카드 1장 뽑기.} |
| `FanTheBrand` | `FAN_THE_BRAND` | `1, Skill, Uncommon, None` | 낙인 흩뿌리기 | {IfUpgraded:show:도탄 3 획득. 각인 1을 획득.\|도탄 2 획득. 각인 1을 획득.} |
| `RicochetShot` | `RICOCHET_SHOT` | `3, Skill, Rare, AnyEnemy` | 도탄 사격 | {IfUpgraded:show:각인 3소모. 격발 1회. 도탄 2 획득. 다음 공격 카드의 비용이 0이 됩니다. (코스트 2).\|각인 3소모. 격발 1회. 도탄 2 획득. 다음 공격 카드의 비용이 0이 됩니다. (코스트 3).} |
| `ReboundNet` | `REBOUND_NET` | `1, Skill, Uncommon, None` | 리바운드 넷 | {IfUpgraded:show:적 1명당 도탄 스택 1 획득. (코스트 0).\|적 1명당 도탄 스택 1 획득.} |
| `ImprintManifestRicochet` | `IMPRINT_MANIFEST_RICOCHET` | `2, Skill, Rare, None` | 현현: 도탄 | {IfUpgraded:show:각인 3소모. 이번 턴 모든 탄환 격발 시 도탄 발동.\|각인 4소모. 이번 턴 모든 탄환 격발 시 도탄 발동.} |
| `TracerConversion` | `TRACER_CONVERSION` | `0, Skill, Uncommon, None` | 추적탄 변환 | {IfUpgraded:show:일반탄 최대 2발을 추적탄으로 변환. 카드 1장 뽑기.\|일반탄 최대 2발을 추적탄으로 변환.} |
| `BallisticCompiler` | `BALLISTIC_COMPILER` | `1, Power, Uncommon, None` | 탄도 컴파일러 | {IfUpgraded:show:매 턴 첫 Tracer 발사 시 카드 1장을 뽑고 각인을 2 획득.\|매 턴 첫 Tracer 발사 시 카드 1장을 뽑고 각인을 1 획득.} |
| `ChainBurst` | `CHAIN_BURST` | `1, Attack, Uncommon, AnyEnemy` | 체인 버스트 | {IfUpgraded:show:각인 2소모. 격발 1회. 사냥개시 ON이면 +2회.\|각인 2소모. 격발 1회. 사냥개시 ON이면 +1회.} |
| `WalkingFire` | `WALKING_FIRE` | `1, Attack, Uncommon, AnyEnemy` | 워킹 파이어 | {IfUpgraded:show:각인 3을 소모합니다. 격발 3회. 이번 카드로 한 번이라도 실탄 발사에 성공하면 추적탄 1발을 장전합니다.\|각인 3을 소모합니다. 격발 2회. 이번 카드로 한 번이라도 실탄 발사에 성공하면 추적탄 1발을 장전합니다.} |
| `BlankFire` | `BLANK_FIRE` | `0, Skill, Uncommon, None` | 블랭크 파이어 | {IfUpgraded:show:각인 1소모. 사냥개시 ON. 격발 1회.\|각인 2소모. 사냥개시 ON. 격발 1회.} |
| `SealSearch` | `SEAL_SEARCH` | `1, Skill, Uncommon, None` | 봉인 추적 | {IfUpgraded:show:봉인탄이 장전되어 있으면 카드 4장 뽑기. 아니면 카드 3장 뽑기.\|봉인탄이 장전되어 있으면 카드 3장 뽑기. 아니면 카드 2장 뽑기.} |
| `SealAmplify` | `SEAL_AMPLIFY` | `1, Skill, Uncommon, None` | 봉인 증폭 | {IfUpgraded:show:장전된 봉인탄의 레벨을 5 올립니다.\|장전된 봉인탄의 레벨을 3 올립니다.} |
| `EmptyTheMagazine` | `EMPTY_THE_MAGAZINE` | `1, Skill, Uncommon, None` | 탄창 비우기 | 장전된 탄환을 모두 제거합니다. 제거한 수만큼 카드를 뽑습니다. 그 후, 이번 턴에는 더 이상 카드를 뽑을 수 없습니다. |
| `OverclockDrum` | `OVERCLOCK_DRUM` | `2, Power, Rare, None` | 오버클록 드럼 | 매 턴 첫 Tracer 발사 시 방아쇠 1회 추가로 당깁니다. 이 효과는 중첩되지 않습니다. |
| `OverclockCharge` | `OVERCLOCK_CHARGE` | `3, Power, Rare, None` | 오버클록 장약 | {IfUpgraded:show:한 턴에 3번째 발사 성공 시 에너지를 1 획득. 선천성.\|한 턴에 3번째 발사 성공 시 에너지를 1 획득.} |
| `ExecutionShot` | `EXECUTION_SHOT` | `2, Attack, Rare, AnyEnemy` | 처형 사격 | {IfUpgraded:show:격발 1회. 대상 체력이 절반 이하면 피해 3배.\|격발 1회. 대상 체력이 절반 이하면 피해 2배.} |
| `ImprintIgnition` | `IMPRINT_IGNITION` | `2, Power, Rare, None` | 각인 점화 | {IfUpgraded:show:각인 소비 카드의 피해량 +2.\|각인 소비 카드의 피해량 +1.} |
| `SealRite` | `SEAL_RITE` | `1, Power, Rare, None` | 봉인의식 | {IfUpgraded:show:파워: 턴 시작 시 봉인탄이 장전되어 있으면 장전된 봉인탄 레벨을 1 올립니다. 사용 시 봉인탄이 없다면 봉인탄 1발을 장전합니다. 선천성.\|파워: 턴 시작 시 봉인탄이 장전되어 있으면 장전된 봉인탄 레벨을 1 올립니다. 사용 시 봉인탄이 없다면 봉인탄 1발을 장전합니다.} |
| `SealReleaseKai` | `SEAL_RELEASE_KAI` | `3, Attack, Rare, AnyEnemy` | 봉인 해방: 개 | {IfUpgraded:show:가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 2회 격발. 보존.\|가장 높은 레벨의 장전된 봉인탄을 발사 칸으로 정렬합니다. 2회 격발.} |
| `SealRampage` | `SEAL_RAMPAGE` | `0, Skill, Rare, None` | 봉인 폭주 | {IfUpgraded:show:봉인탄이 장전되어 있을 때만 사용 가능. 가장 높은 봉인 레벨 1 감소. [gold]에너지[/gold] 1 획득. 카드 1장 뽑기.\|봉인탄이 장전되어 있을 때만 사용 가능. 가장 높은 봉인 레벨 2 감소. [gold]에너지[/gold] 1 획득.} |
| `SealInsight` | `SEAL_INSIGHT` | `1, Skill, Rare, None` | 봉인 통찰 | 카드 {Draw}장을 뽑습니다. 봉인탄이 장전되어 있으면 +1, 봉인 레벨이 7 이상이면 추가로 +1 더 뽑습니다. |
| `SealBarrier` | `SEAL_BARRIER` | `0, Skill, Rare, Self` | 봉인 방벽 | 방어도 5 획득. 봉인탄이 있으면 최고 봉인 레벨 × 2만큼 추가 획득하고 해당 봉인탄 레벨을 1 올립니다. |

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
  - `TagBurst` 설명(기본): 격발 1회, 발사 성공 시 추가 {Damage:diff()} + 각인당 1 피해, 도탄 1 획득.
  - `TagBurst` 설명(업그레이드): 격발 1회, 발사 성공 시 추가 {Damage:diff()} + 각인당 2 피해, 도탄 2 획득.
- `OverclockDrum`은 효과를 비중첩(single)로 고정해 오해 소지가 있는 가짜 중첩을 제거.
- `SteadyAim` 상향: 기본 도탄 2, 업그레이드 도탄 3.
- `FanTheBrand` 상향: 기본 도탄 2 + 각인 1, 업그레이드 도탄 3 + 각인 1.


## 2026-03-27 Seal 축 정리 패스

- `SealOpen` 제거: 카드 풀/활성 설계에서 삭제.
- `GrandChamber` 제거: 카드 풀에서 삭제(중복 정렬 역할 축소).
- `SealSearch` 재설계: 정렬 제거, 봉인탄 장전 여부 기반 드로우 지원 카드로 변경.
- `GrandRite` 재설계: 단일 봉인탄 성장(+2/+3) + 내부 정렬 + 즉시 1회 해방.
- `SealReleaseKai` 정렬 내장: 최고 레벨 봉인탄 정렬 후 2회 발사.
- `RicochetSeal` 재설계: 도탄 보상 제거, 봉인탄 직접 격발 성공 시 드로우 보상(1/2).
- `SealRampage` 재설계: 최고 봉인 레벨 일부 소모(-2/-1) 후 에너지 1 획득, 업그레이드 시 카드 1장 드로우.
