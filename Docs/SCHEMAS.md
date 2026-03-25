# Schemas

게임 내 핵심 데이터 구조 정의.

---

## Tile

```csharp
public enum TileState
{
    Hidden,     // 미공개
    Revealed,   // 공개됨
    Exploded    // 지뢰 폭발
}

public struct TileData
{
    public Vector2Int position;   // 그리드 좌표
    public bool isMine;           // 지뢰 여부
    public int adjacentMines;     // 인접 지뢰 수 (0~8)
    public TileState state;       // 현재 상태
}
```

## Map

```csharp
public class MapConfig
{
    // 맵 크기 (세로)
    public int height;                    // 고정 (예: 9칸)

    // 지뢰 밀도 스케일링
    public DensityThreshold[] densityTable;
}

public struct DensityThreshold
{
    public int distanceFrom;    // 시작 거리
    public int distanceTo;      // 종료 거리
    public float density;       // 지뢰 밀도 (0.0 ~ 1.0)
}

// 기본값
// { 0,   50,  0.15f }   15%
// { 51,  150, 0.22f }   22%
// { 151, 300, 0.28f }   28%
// { 301, max, 0.33f }   33%
```

## Player

```csharp
public class PlayerState
{
    public Vector2Int gridPosition;   // 현재 그리드 위치
    public int currentDistance;       // 현재 전진 거리
    public int maxDistance;           // 이번 판 최대 도달 거리
    public bool isAlive;
}
```

## Gimmick

```csharp
public enum GimmickCategory
{
    InfoDistortion,   // 정보 왜곡
    Vision,           // 시야
    Psychological     // 심리
}

// ScriptableObject로 정의
public class GimmickData : ScriptableObject
{
    public string gimmickName;
    public string description;
    public int difficulty;              // 1~5
    public GimmickCategory category;
    public float duration;              // 지속 시간 (초), 0이면 영구
    public Sprite icon;                 // UI 표시용 아이콘
}

public interface IGimmick
{
    void Activate();     // 기믹 발동
    void Deactivate();   // 기믹 해제
    void OnTick();       // 매 턴 호출 (턴 기반 기믹용)
}
```

### 기믹 레벨 계산

```csharp
// 현재 레벨
int level = Mathf.FloorToInt(currentDistance / 50f);

// 등장 가능 기믹 = difficulty <= level
List<GimmickData> pool = allGimmicks
    .Where(g => g.difficulty <= level)
    .ToList();
```

## Record

```csharp
public class RecordData
{
    public int bestDistance;        // 최고 기록
    public int totalDeaths;        // 총 사망 횟수
    public int totalGamesPlayed;   // 총 플레이 횟수
}

// 저장: PlayerPrefs (로컬)
// Key: "BestDistance", "TotalDeaths", "TotalGamesPlayed"
```

## Difficulty Curve

```
distance    밀도    기믹 레벨    체감 난이도
─────────────────────────────────────────
0~50        15%     0 (없음)     ★☆☆☆☆  튜토리얼 구간
51~100      22%     1            ★★☆☆☆  기믹 입문
101~150     22%     2            ★★★☆☆  판단력 시험
151~250     28%     3~5          ★★★★☆  본격 고난이도
250~300     28%     5            ★★★★★  지옥
300+        33%     5+           ★★★★★  극한
```
