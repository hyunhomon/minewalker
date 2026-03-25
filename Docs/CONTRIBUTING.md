# Contributing Guide

## Git Workflow

### 브랜치 구조

```
main              ← 안정 빌드 (릴리즈 가능한 상태만)
└─ develop        ← 개발 통합 브랜치
     ├─ Feat/#10-player-movement
     ├─ Art/#15-character-idle-sprite
     ├─ Fix/#23-wall-collision
     └─ ...
```

- `main`: 릴리즈 가능한 안정 버전만. 직접 푸시 금지
- `develop`: 모든 피처 브랜치의 머지 대상. PR로만 머지

### 브랜치 네이밍

GitHub 이슈 기반으로 브랜치를 생성합니다.

```
<Category>/#<이슈번호>-<간단한-설명>
```

| Category | 용도 | 예시 |
|----------|------|------|
| Feat | 기능 구현 | `Feat/#12-player-movement` |
| Art | 아트 에셋 | `Art/#15-character-idle-sprite` |
| Level | 레벨/맵 작업 | `Level/#18-stage1-tilemap` |
| Sound | 사운드 | `Sound/#21-bgm-main-theme` |
| Fix | 버그 수정 | `Fix/#23-wall-collision` |
| Refactor | 리팩토링 | `Refactor/#25-input-system` |
| Infra | 빌드/CI/설정 | `Infra/#2-git-lfs-setup` |

### 이슈 작성 규칙

```
[<Category>] 설명
```

예시: `[Feat] Player 4방향 타일 이동`, `[Art] 캐릭터 idle 스프라이트`

이슈 본문에 다음을 명시:
- **담당자**
- **관련 씬/프리팹** (충돌 방지)
- **완료 조건**

### 커밋 메시지

```
<type>: <설명>
```

| type | 용도 |
|------|------|
| feat | 새 기능 |
| fix | 버그 수정 |
| art | 아트 에셋 추가/수정 |
| sound | 사운드 추가/수정 |
| refactor | 리팩토링 |
| docs | 문서 |
| chore | 빌드, 설정 등 |

### PR 규칙

- `develop`으로만 PR
- 최소 1명 리뷰 후 머지
- 씬/프리팹 변경이 포함된 PR은 관련 담당자 리뷰 필수

---

## 충돌 방지

### 씬 담당제

동일한 씬 파일을 두 명 이상이 동시에 수정하면 머지가 거의 불가능합니다.

- 각 씬에는 **담당자 1명**을 지정
- 씬에 오브젝트를 추가할 때는 **프리팹으로 만들어서** 씬 담당자에게 배치 요청
- 이슈에 관련 씬/프리팹을 반드시 명시

### 프리팹 중심 작업

```
프로그래머 → Enemy.prefab 제작
아티스트   → enemy_walk.png 스프라이트 추가
씬 담당자  → Enemy.prefab을 Stage1.unity에 배치
```

---

## Unity 작업 주의사항

- **파일 이동/이름 변경은 반드시 Unity 에디터 안에서**. 탐색기에서 하면 .meta 파일 참조가 깨짐
- **커밋 전 Unity에서 Ctrl+S**. 저장 안 하면 변경사항이 디스크에 안 쓰여짐
- **Play 모드에서 수정한 값은 Play 끝나면 사라짐**
- **에디터 닫고 커밋하면 가장 깔끔** (임시 파일 방지)

---

## Git LFS

바이너리 파일은 Git LFS로 관리됩니다. `.gitattributes`에 정의된 확장자는 자동으로 LFS를 탑니다.

- LFS 대상: png, jpg, psd, tga, wav, mp3, ogg, fbx, ttf, otf 등
- 팀원은 `git lfs install` 한번 실행 필요 (Git LFS 클라이언트 설치 필요)
