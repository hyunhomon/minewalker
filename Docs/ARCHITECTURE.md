# Architecture

## 프로젝트 디렉터리 구조

```
Assets/
├── Scenes/              # 씬 파일
│   ├── Main.unity       # 메인 게임 씬
│   └── UI.unity         # UI 전용 씬 (Additive)
├── Scripts/
│   ├── Core/            # 게임 핵심 시스템
│   │   ├── GameManager.cs
│   │   ├── TileSystem.cs
│   │   └── MapGenerator.cs
│   ├── Player/          # 플레이어 관련
│   │   ├── PlayerController.cs
│   │   └── PlayerInput.cs
│   ├── Gimmick/         # 기믹 시스템
│   │   ├── GimmickManager.cs
│   │   └── Gimmicks/    # 개별 기믹 구현
│   ├── UI/              # UI 스크립트
│   │   ├── HUDController.cs
│   │   ├── DeathScreen.cs
│   │   └── RecordDisplay.cs
│   └── Utils/           # 공용 유틸리티
├── Prefabs/
│   ├── Player/
│   ├── Tiles/
│   ├── Effects/
│   └── UI/
├── Sprites/             # 도트 아트 (16x16)
│   ├── Player/
│   ├── Tiles/
│   └── Effects/
├── Audio/
│   ├── BGM/
│   └── SFX/
└── Settings/            # URP, 렌더링 설정
```

## 시스템 구조

```
GameManager (싱글톤)
├── MapGenerator       # 무한 맵 생성
│   ├── 지뢰 배치
│   ├── 숫자 계산
│   └── 안전 경로 보장
├── TileSystem         # 타일 상태 관리
│   ├── 타일 공개/숨김
│   └── 타일 데이터 저장
├── PlayerController   # 이동 + 입력
│   ├── 4방향 타일 이동
│   ├── 전진/후퇴 판정
│   └── 사망 처리
├── GimmickManager     # 기믹 스폰 + 관리
│   ├── 레벨 기반 풀링
│   └── 기믹 활성화/해제
├── RecordManager      # 거리 기록
│   ├── 현재 거리 추적
│   ├── 최고 기록 저장 (로컬)
│   └── 리더보드 (추후)
└── UIManager          # UI 제어
    ├── HUD (거리, 기믹 표시)
    ├── 사망 화면
    └── 메인 메뉴
```

## 핵심 시스템 상세

### MapGenerator

무한 맵 생성 알고리즘. 플레이어가 전진하면 새 열을 실시간 생성.

- 지뢰 밀도 스케일링: 거리에 따라 15% → 33%
- **안전 경로 보장**: 생성 시 최소 1개 통과 가능 경로 검증
- 뒤쪽 열은 일정 거리 이후 해제 (메모리 관리)

### GimmickManager

거리 기반 레벨 시스템으로 기믹 등장 관리.

```
level = floor(distance / 50)
등장 가능 = difficulty <= level 인 기믹 풀
```

- 기믹은 ScriptableObject로 정의
- 각 기믹은 공통 인터페이스(IGimmick) 구현
- 카테고리: 정보 왜곡 / 시야 / 심리 (총 15종)

### TileSystem

타일맵 기반 타일 상태 관리.

- Unity Tilemap 사용
- 타일 상태: Hidden / Revealed / Flagged / Exploded
- 숫자 표시는 타일 위 오버레이 텍스트

## 담당 분리 가이드

| 시스템 | 주요 파일 | 비고 |
|--------|-----------|------|
| 맵 생성 | `Scripts/Core/MapGenerator.cs` | 리드 개발 담당 |
| 기믹 | `Scripts/Gimmick/` | 리드 개발 담당, 개별 기믹은 분담 가능 |
| 플레이어 | `Scripts/Player/` | 개발 A 담당 |
| UI | `Scripts/UI/`, `Prefabs/UI/` | 개발 B 담당 |
| 아트 | `Sprites/` | 아트 A 담당 |
| 사운드 | `Audio/` | 사운드 A 담당 |
