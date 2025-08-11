# 🛠️ 플레이어 (Player)


## 목차

- [🌙 OverView 🌙](#overview)
- [🌴 Architecture 🌴](#architecture)
- [⚙️ Functions ⚙️](#function)
- [📚 Libraries 📚](#library)
- [🧗 Technical Advantages 🧗](#pros)
- [🤖 FSM (Finite State Machine) 🤖](#fsm)

---

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 플레이어를 구현하기 위해 적용된 기술 스택을 소개합니다.
핵심 아키텍처와 주요 시스템별 구현 내용에 대해 알아볼 수 있으며 활용 기술 및 라이브러리를 알 수 있습니다.
또한 이러한 기술을 사용했을 시 따라오는 장점에 대해 알 수 있습니다.

---

<a name="architecture"></a>
## 1. 핵심 아키텍처
- **패턴**:  
  - 상태 패턴(State Pattern) → `PlayerStateMachine` + `BaseState` 기반
  - 컴포넌트 기반(Entity-Component)
  - DI(Dependency Injection) 유사 패턴을 통한 CoreManager 중심 모듈 참조
- **비동기 처리**: Cysharp **UniTask**를 통한 코루틴 대체 및 취소 토큰 기반 동작 제어
- **애니메이션**: Animator 파라미터 해시 관리 + 무기별 전용 Animator 연동
- **카메라**: Cinemachine (POV, HardLock, FoV Transition 등)


<a name="function"></a>
## 2. 주요 시스템별 구현 내용

### (1) Player.cs – 플레이어 메인 컨트롤 허브
- `CharacterController` 기반의 이동/충돌 처리
- **카메라 포인트 및 피벗 구조**를 통한 1인칭 시점 제어
- `AnimationData` 초기화 및 Animator 파라미터 관리
- 플레이어 핵심 컴포넌트(PlayerCondition, PlayerWeapon, PlayerInventory 등) 초기화
- 상태 머신(`PlayerStateMachine`) 구동 및 상태 전환 관리
- Animation Event 기반 발자국, 점프, 착지 사운드 재생 로직 구현

### (2) PlayerCondition.cs – 플레이어 상태 관리
- **스탯/게이지 시스템**: HP, 스태미나, 실드, 포커스, 인스팅트 등
- **버프/디버프 처리**: 무게, 스킬, 아이템에 따른 이동속도 가변
- **전투 로직**: 공격 처리, 무기 교체, 조준, 재장전
- **스킬 로직**: Focus(슬로우 모션), Instinct(속도 증가 + 하이라이트) 구현
- UniTask + `CancellationTokenSource`를 활용한 **행동 중단/취소 가능 로직**
- UI와 실시간 연동(InGameUI, SkillOverlayUI, WeaponUI 업데이트)

### (3) PlayerInventory.cs – 인벤토리 & 아이템 사용 시스템
- **직렬화 딕셔너리**(`SerializedDictionary<ItemType, BaseItem>`) 기반 아이템 보관
- 아이템 사용/선택/충전 로직
- QuickSlot UI 오픈(길게 누름 감지) 및 플레이어 컨트롤 잠금 처리

### (4) PlayerWeapon.cs – 무기 관리 시스템
- 무기별 Animator 및 Script 객체 매핑
- 데이터 로드 시 무기 잠금/해금 상태 초기화
- 부품 장착/해제(PartType 단위)
- 무기 제작 및 부품 해금 기능

### (5) BaseState.cs – 플레이어 상태 베이스 로직
- 상태 전환에 필요한 **입력 처리 콜백** 바인딩/해제
- 이동/회전 로직:
  - 카메라 기준 전방·우측 벡터 기반 방향 계산
  - 이동 속도 보간(Lerp) + 이동 모드별 속도 가변
- 무기 발사, 조준, 재장전, 무기 전환, 아이템 사용, 상호작용 등의 공통 처리
- 마우스 감도/이동속도에 스킬·아이템·무기 무게 패널티 반영


<a name="library"></a>
## 3. 활용 기술 및 라이브러리

| 기술/라이브러리 | 활용 목적 |
|-----------------|----------|
| **Unity CharacterController** | 플레이어 충돌 및 이동 처리 |
| **Cinemachine** | 카메라 제어, FoV 전환, 시점 회전 |
| **UniTask (Cysharp)** | 경량 비동기 처리, 취소 토큰 기반 실행 중단 |
| **DG.Tweening (DOTween)** | 카메라/오브젝트 애니메이션, 부드러운 전환 |
| **SerializedDictionary (AYellowpaper)** | 직렬화 가능한 딕셔너리 형태의 데이터 보관 |
| **Animator** | 무기·캐릭터 애니메이션 제어 |
| **AudioLowPassFilter** | HP 비율 기반 사운드 필터링 효과 |
| **ScriptableObject** | 무기·아이템·스탯 데이터 관리 |


<a name="pros"></a>
## 4. 기술적 강점
- **모듈화**: 상태, 인벤토리, 무기, 컨디션 로직을 분리하여 유지보수 용이
- **확장성**: 새 무기/아이템/스킬 추가 시 최소 변경으로 확장 가능
- **성능 최적화**: UniTask 활용으로 GC 부담 완화 + Update 최소화
- **게임플레이 유연성**: 스킬·아이템·무기 상태에 따른 속도/감도 동적 변화

---

# Extra Info.
<a name="fsm"></a>
## 🤖 FSM (Finite State Machine)

### 개념

FSM은 플레이어 행동을 여러 상태(State)로 나누고,  
각 상태 간의 전환(Transition)을 체계적으로 관리하는 설계 기법입니다.  
각 상태는 명확한 역할과 행동을 가지고 있어, 복잡한 동작을 명확하고 쉽게 구현할 수 있습니다.

### 도입 이유

- 플레이어 행동 로직을 깔끔하게 분리하여 유지보수성 향상
- 상태별로 명확한 동작 정의로 코드 복잡도 감소
- 다양한 플레이어 행동 확장 및 제어에 용이

### 주요 메서드 및 기능

| 메서드                                                                                                                                   | 기능                  |
|---------------------------------------------------------------------------------------------------------------------------------------|---------------------|
| [PlayerStateMachine](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/PlayerStateMachine.cs#L30) | 이 스크립트에서는 Player가 가질 수 있는 모든 State가 선언되는 곳이며 Player의 현재 State를 저장하는 곳입니다. ([StateMachine](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)을 상속받습니다.) |
| [ChangeState](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)                                                                                                                     | FSM의 현재 상태를 다른 상태로 전환하는 함수입니다. |
| [HandleInput](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)                                                                                                                           | 유저의 키 입력을 처리하는 함수입니다. |
| [Update](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)                                                                                                                           | 매 프레임마다 현재 상태에 따른 동작을 실행합니다. |
| [LateUpdate](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)                                                                                                                             | 매 프레임마다 Update보다 느린 타이밍에 현재 상태에 따른 동작을 실행합니다. |
| [PhysicsUpdate](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/StateMachine.cs)                                                                                                                             | 매 FixedTime 마다 현재 상태에 계산이 필요한 물리적 작용을 처리하는 함수입니다. |

| 메서드                                                                                                                                   | 기능                  |
|---------------------------------------------------------------------------------------------------------------------------------------|---------------------|
| [BaseState](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/States/BaseState.cs) | 이 스크립트는 모든 State의 기본 골자가 되는 부모 클래스입니다. 모든 State는 BaseState를 상속하여 기존에 존재하는 함수를 오버라이딩 하는 방식으로 작동합니다. |
| Move | 플레이어의 움직임을 구현하는 함수입니다. |
| Rotate | 플레이어의 몸체를 회전시키는 함수입니다. |
| OnMoveCanceled | 유저의 Move 입력이 중단되었을 때 역할을 수행하는 함수입니다. |
| On (Jump, Run, Crouch) Started | 유저의 키 입력을 받아 각자의 역할에 맞게 점프, 뛰기, 앉기를 수행하는 함수입니다.  |
| On (Aim, Fire) Started | 유저의 키 입력을 받아 조준하거나 총을 발사하는 기능을 수행하는 함수입니다. |
| OnSwitch (ToMain, ToSecondary, ToGrenade, ToHackGun, ByScroll) | 주 무기, 보조 무기, 유탄기, 해킹건과 같이 무기를 교체하는 함수로 키 입력을 받을 시 수행합니다. |
