# Gunslinger 전체 카드 사양 (현재 코드 기준)

- 기준 시점: 2026-03-26
- 소스: `src/GunslingerMod/Models/CardPools/GunslingerCardPool.cs` + 각 카드 클래스 + `GunslingerMod/localization/kor/cards.json`

| 클래스 | 키 | 코스트/타입/희귀도/타겟 | 카드명(ko) | 카드 설명(ko) |
|---|---|---|---|---|
| `Shoot` | `SHOOT` | `1, Attack, Basic, AnyEnemy` | 사격 | {IfUpgraded:show:격발 1회. 실탄이면 {Damage:diff()} 피해(+3)를 주고 각인 1을 얻습니다. 빈 약실이면 카드 1장을 뽑습니다.\|격발 1회. 실탄이면 {Damage:diff()} 피해를 주고 각인 1을 얻습니다. 빈 약실이면 카드 1장을 뽑습니다.} |
| `DefendGunslinger` | `DEFEND_GUNSLINGER` | `(확인 필요)` | 방어 | {Block:diff()} 방어도를 획득. |
| `Reload` | `RELOAD` | `0, Skill, Common, None` | 장전 | {IfUpgraded:show:일반탄 2발 장전(최대 6). 카드 1장 뽑기.\|일반탄 2발 장전(최대 6).} |
| `TakeCover` | `TAKE_COVER` | `1, Skill, Common, None` | 엄폐 | {IfUpgraded:show:{Block:diff()} 방어도를 얻습니다. 빈 약실 하나당 방어도를 +1 얻습니다.\|{Block:diff()} 방어도를 얻습니다. 빈 약실 하나당 방어도를 +1 얻습니다. 이번 턴 [재장전 불가] 상태가 됩니다.} |
| `Evasion` | `EVASION` | `1, Skill, Common, None` | 회피 | 이번 턴 다음에 받는 피해를 절반으로 만듭니다. {IfUpgraded:show:(코스트 0).\|} |
| `EchoNote` | `ECHO_NOTE` | `0, Skill, Common, None` | 에코 노트 | {IfUpgraded:show:각인 1소모. 카드 2장 뽑기.\|각인 1소모. 카드 1장 뽑기.} |
| `QuickRack` | `QUICK_RACK` | `1, Attack, Common, AnyEnemy` | 퀵 랙 | 각인 1을 소모합니다. 추적탄 2발을 장전합니다. {IfUpgraded:show:사냥개시 상태면 격발 2회, 아니면 카드 1장 뽑기.\|사냥개시 상태면 격발 1회, 아니면 카드 1장 뽑기.} |
| `HotChamber` | `HOT_CHAMBER` | `1, Skill, Common, None` | 핫 챔버 | 추적탄 2발을 장전합니다. 사냥개시 상태면 카드 2장을 뽑고, 아니면 카드 1장을 뽑습니다. {IfUpgraded:show:\|기본: 각인 1 소모.} |
| `Panning` | `PANNING` | `3, Attack, Common, AnyEnemy` | 패닝 | {IfUpgraded:show:장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해. 보존.\|장전된 탄환을 순서대로 모두 격발. 격발마다 {Damage:diff()} 피해.} |
| `SprayFire` | `SPRAY_FIRE` | `2, Attack, Common, None` | 난사 | 장전된 탄환을 무작위 적에게 모두 격발. 총 {Damage:diff()} 피해. 격발 수만큼 [gold]각인[/gold] 획득(최대 3). |
| `SealLoad` | `SEAL_LOAD` | `1, Skill, Common, None` | 봉인탄 장전 | 봉인탄 1발 장전(최대 1). 이미 장전되어 있으면 봉인 레벨 +1. {IfUpgraded:show:방어도 8 획득.\|방어도 5 획득.} |
| `SigilGuard` | `SIGIL_GUARD` | `1, Skill, Common, Self` | 시질 가드 | {IfUpgraded:show:방어도 12 획득. 봉인 레벨당 방어도 +1.\|방어도 8 획득. 봉인 레벨당 방어도 +1.} |
| `EtchedTracer` | `ETCHED_TRACER` | `0, Skill, Common, None` | 에칭 트레이서 | 격발 1회. 추적탄 1발 장전. {IfUpgraded:show:대신 추적탄 2발 장전.\|} |
| `ImprintSqueeze` | `IMPRINT_SQUEEZE` | `0, Skill, Uncommon, None` | 각인 압착 | 카드 1장 버림. 각인 {IfUpgraded:show:3\|2}획득. |
| `ImprintCompression` | `IMPRINT_COMPRESSION` | `1, Power, Uncommon, None` | 각인 압축 | 턴 시작 시 각인이 {IfUpgraded:show:3\|4} 이상이면 카드 1장 뽑기. |
| `FanTheBrand` | `FAN_THE_BRAND` | `1, Skill, Uncommon, None` | 낙인 흩뿌리기 | 도탄 스택을 {IfUpgraded:show:2\|1} 획득. 각인 1을 획득. |
| `RicochetShot` | `RICOCHET_SHOT` | `3, Skill, Rare, None` | 도탄 사격 | 각인 3소모. 도탄 2 획득. 다음 사격 카드의 비용이 0이 됩니다. {IfUpgraded:show:(코스트 2).\|(코스트 3).} |
| `ReboundNet` | `REBOUND_NET` | `1, Skill, Uncommon, None` | 리바운드 넷 | 적 1명당 도탄 스택 1 획득. {IfUpgraded:show:(코스트 0).\|} |
| `ImprintManifestRicochet` | `IMPRINT_MANIFEST_RICOCHET` | `2, Skill, Rare, None` | 현현: 도탄 | 각인 {IfUpgraded:show:3\|4}소모. 이번 턴 모든 탄환 격발 시 도탄 발동. |
| `TracerConversion` | `TRACER_CONVERSION` | `0, Skill, Uncommon, None` | 추적탄 변환 | 일반탄 최대 2발을 추적탄으로 변환. {IfUpgraded:show:카드 1장 뽑기.\|} |
| `BallisticCompiler` | `BALLISTIC_COMPILER` | `1, Power, Uncommon, None` | 탄도 컴파일러 | 매 턴 첫 Tracer 발사 시 카드 1장을 뽑고 각인을 {IfUpgraded:show:2\|1} 획득. |
| `ChainBurst` | `CHAIN_BURST` | `1, Attack, Uncommon, AnyEnemy` | 체인 버스트 | 각인 2소모. 격발 1회. 사냥개시 ON이면 {IfUpgraded:show:+2회.\|+1회.} |
| `WalkingFire` | `WALKING_FIRE` | `1, Attack, Uncommon, AnyEnemy` | 워킹 파이어 | 각인 3을 소모합니다. {IfUpgraded:show:격발 3회.\|격발 2회.} 이번 카드로 한 번이라도 실탄 발사에 성공하면 추적탄 1발을 장전합니다. |
| `BlankFire` | `BLANK_FIRE` | `0, Skill, Uncommon, None` | 블랭크 파이어 | 각인 {IfUpgraded:show:1\|2}소모. 사냥개시 ON. 격발 1회. |
| `SealSearch` | `SEAL_SEARCH` | `1, Skill, Uncommon, None` | 봉인 추적 | 카드를 {IfUpgraded:show:3\|2}장 뽑습니다. 가장 높은 레벨의 봉인탄을 발사 칸으로 이동시킵니다. |
| `SealAmplify` | `SEAL_AMPLIFY` | `1, Skill, Uncommon, None` | 봉인 증폭 | {IfUpgraded:show:장전된 모든 봉인탄의 레벨을 4 올립니다.\|봉인탄 1발을 선택해 레벨을 3 올립니다.} |
| `SealResonance` | `SEAL_RESONANCE` | `1, Attack, Uncommon, AnyEnemy` | 봉인 공명 | 가장 높은 봉인탄 레벨의 2배 피해. |
| `EmptyTheMagazine` | `EMPTY_THE_MAGAZINE` | `1, Skill, Uncommon, None` | 탄창 비우기 | 탄환을 모두 제거합니다. 제거한 수만큼 뽑습니다. 이번 턴 더 이상 카드를 뽑을 수 없습니다. |
| `OverclockDrum` | `OVERCLOCK_DRUM` | `2, Power, Rare, None` | 오버클록 드럼 | 매 턴 첫 Tracer 발사 시 방아쇠 1회 추가로 당깁니다. |
| `OverclockCharge` | `OVERCLOCK_CHARGE` | `3, Power, Rare, None` | 오버클록 장약 | 한 턴에 3번째 발사 성공 시 에너지를 1 획득. {IfUpgraded:show:선천성.\|} |
| `ExecutionShot` | `EXECUTION_SHOT` | `2, Attack, Rare, AnyEnemy` | 처형 사격 | 격발 1회. 대상 체력이 절반 이하면 피해 {IfUpgraded:show:3배\|2배}. 각인 3 획득. |
| `ImprintIgnition` | `IMPRINT_IGNITION` | `2, Power, Rare, None` | 각인 점화 | 각인 소비 카드의 피해량 {IfUpgraded:show:+2\|+1}. |
| `SealRite` | `SEAL_RITE` | `1, Power, Rare, None` | 봉인의식 | 파워: 턴 시작 시 봉인탄이 장전되어 있으면 봉인 레벨을 1 올립니다. 사용 시 봉인탄이 없다면 봉인탄 1발을 장전합니다. {IfUpgraded:show:선천성.\|} |
| `SealOpen` | `SEAL_OPEN` | `1, Skill, Rare, None` | 봉인 개방 | 가장 높은 레벨의 봉인탄을 발사 칸으로 이동시킵니다. {IfUpgraded:show:그 봉인탄 레벨을 2 올립니다.\|} |
| `SealReleaseKai` | `SEAL_RELEASE_KAI` | `3, Attack, Rare, AnyEnemy` | 봉인 해방: 개 | 장전된 봉인탄을 격발합니다. 해당 봉인탄을 2번 발사. {IfUpgraded:show:보존.\|} |
| `SealRampage` | `SEAL_RAMPAGE` | `0, Skill, Rare, None` | 봉인 폭주 | 봉인탄이 장전되어 있을 때만 사용 가능. 모든 봉인 레벨을 절반으로 만들고 [gold]에너지[/gold] 1 획득. |
| `SealInsight` | `SEAL_INSIGHT` | `1, Skill, Rare, None` | 봉인 통찰 | 카드 {Draw}장을 뽑습니다. 봉인탄이 장전되어 있으면 +1, 봉인 레벨이 7 이상이면 추가로 +1 더 뽑습니다. |
| `SealBarrier` | `SEAL_BARRIER` | `0, Skill, Rare, Self` | 봉인 방벽 | 방어도 5 획득. 봉인탄이 있으면 최고 봉인 레벨 × 2만큼 추가 획득. |
