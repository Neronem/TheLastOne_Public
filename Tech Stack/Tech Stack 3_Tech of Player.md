# 🛠️ 플레이어 (Player)


## 목차

- [🌙 OverView 🌙](#overview)
- [🤖 FSM (Finite State Machine) 🤖](#fsm)

---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 플레이어를 구현하기 위해 적용된 기술 스택을 소개합니다.

<br>

---

<br>

<a name="fsm"></a>
## 🤖 FSM (Finite State Machine)

### 개념

FSM은 플레이어 행동을 여러 상태(State)로 나누고,  
각 상태 간의 전환(Transition)을 체계적으로 관리하는 설계 기법입니다.  
각 상태는 명확한 역할과 행동을 가지고 있어, 복잡한 동작을 명확하고 쉽게 구현할 수 있습니다.

### BaseState.cs – 플레이어 상태 베이스 로직
- 상태 전환에 필요한 **입력 처리 콜백** 바인딩/해제
- 이동/회전 로직:
  - 카메라 기준 전방·우측 벡터 기반 방향 계산
  - 이동 속도 보간(Lerp) + 이동 모드별 속도 가변
- 무기 발사, 조준, 재장전, 무기 전환, 아이템 사용, 상호작용 등의 공통 처리
- 마우스 감도/이동속도에 스킬·아이템·무기 무게 패널티 반영

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

<br>

---
