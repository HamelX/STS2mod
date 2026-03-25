# Excel v2 기준 정리 메모

기준 파일: `CARD_POOL_KO_v2.xlsx`

## 이번에 반영한 정리
- 활성 카드풀을 엑셀 v2 기준 38장으로 정렬
- `GunslingerCardPool.cs`에서 아래 카드를 **활성 카드풀에서 제외**
  - `CrossfireRhythm`
  - `TagBurst`
  - `GrandRite`
- `SealBarrier`를 **활성 카드풀에 추가**
- `GunslingerMod/localization/kor/cards.json`에 누락된 `SEAL_BARRIER.title` 추가

## 참고
아래 카드 클래스 파일은 소스에 남아 있지만 현재 활성 카드풀에는 포함되지 않음.
- `CrossfireRhythm.cs`
- `TagBurst.cs`
- `GrandRite.cs`

즉, 지금 상태는 **엑셀 v2 기준 카드풀과 활성 등록부를 맞춘 상태**이며,
구상/폐기/보관 후보 클래스는 추후 별도로 정리하면 됨.
