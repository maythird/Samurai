# Samurai

Unity로 제작한 2D 사무라이 vs 데몬 전투 프로토타입입니다.

## 개요

사무라이가 시작 위치에서 전투 위치까지 달려온 뒤, 버튼 UI를 통해 데몬과 전투하는 씬입니다.  
애니메이션 상태 머신, 대리자(delegate) 기반 이벤트, 이펙트 타이밍 동기화를 학습 목적으로 구현했습니다.

## 게임 흐름

```
게임 시작
  → Samurai가 시작 위치에서 원점으로 Run 상태로 이동
  → 도착 시 Idle 전환 + Attack 버튼 등장

Attack 버튼 클릭
  → Samurai Attack 애니메이션 재생
  → 애니메이션 특정 타이밍에 슬래시 이펙트 생성 + Demon TakeDamage
  → Demon Hit 애니메이션 재생 (공격 모션 남은 시간만큼 유지)
  → HP가 0이 되면 Demon Death 애니메이션 재생 후 사라짐
  → Recovery 버튼 등장 → 클릭 시 Demon 부활
```

## 스크립트 구조

| 스크립트 | 역할 |
|----------|------|
| `GameMain` | 전체 게임 흐름 관리. 버튼 이벤트 처리, UI 갱신, 이펙트 요청 |
| `Samurai` | 상태(Idle / Run / Attack) 관리, 공격 타이밍 이벤트 발행 |
| `Demon` | 상태(Idle / Hit / Death) 및 HP 관리, 부활 처리 |
| `FxManager` | 슬래시 이펙트 생성 및 파티클 종료 후 자동 제거 |

## 이벤트 구조 (대리자)

```
Samurai.onStateChanged    → GameMain: 버튼 표시 / 숨김
Samurai.onAttackHit       → GameMain: TakeDamage 호출 + 이펙트 생성
Demon.onStateChanged      → GameMain: Death 코루틴 시작
Demon.onHpChanged         → GameMain: HP 게이지 갱신 / HP 0 시 게이지 즉시 숨김
```

## 주요 구현 포인트

- **공격 타이밍 동기화** — `attackFxTiming`(0~1) 값으로 Attack 애니메이션의 특정 `normalizedTime`에 슬래시 이펙트와 Hit 상태를 동시에 발동
- **Hit 지속시간 동기화** — 이펙트 발동 시점부터 공격 애니메이션이 끝날 때까지의 남은 시간을 Demon의 Hit 상태 지속시간으로 전달
- **HP 게이지 추적** — `LateUpdate`에서 Demon 스프라이트 상단에 UI 게이지를 월드→스크린 좌표 변환으로 고정
- **Death 처리** — 애니메이션 `normalizedTime >= 0.95f` 도달 시 Animator 비활성화 후 오브젝트 숨김

## Animator 파라미터

### Samurai (`IDLE_0.controller`)
| State | int 값 |
|-------|--------|
| Idle  | 0 |
| Attack | 1 |
| Run | 2 |

### Demon (`demon_idle_1_0.controller`)
| State | 파라미터 |
|-------|----------|
| Idle  | State = 0 |
| Hit   | State = 1 |
| Death | Trigger: Death |

## Inspector 설정 항목

### GameMain
| 필드 | 설명 |
|------|------|
| `Attack Damage` | 공격 1회당 데미지 (기본값: 10) |
| `Hp Gague Head Offset` | 게이지와 스프라이트 상단 사이 여백 |
| `Canvas` | UI Canvas 오브젝트 연결 필요 |

### Samurai
| 필드 | 설명 |
|------|------|
| `Run Speed` | 이동 속도 (기본값: 3) |
| `Attack Fx Timing` | 이펙트 발동 타이밍 (0~1, 기본값: 0.5) |

### Demon
| 필드 | 설명 |
|------|------|
| `Max Hp` | 최대 HP (기본값: 100) |

## 개발 환경

- Unity 6 (URP)
- C#

