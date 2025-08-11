# 🛠️ 플레이어 (Player)


## 목차

- [🌙 OverView 🌙](#overview)
- [🤖 FSM (Finite State Machine) 🤖](#fsm)
- [🎮 캐릭터 컨트롤러 (Character Controller) 🎮](#character-controller)

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

</br>

<a name="character-controller"></a>
## 🎮 Character Controller

### 개념

Unity에서 Rigidbody 기반 물리 시뮬레이션 없이 간단하고 안정적인 충돌 처리를 제공하는 전용 이동 콜리전 시스템입니다. 주로 1인칭/3인칭 플레이어 이동, AI 이동 등에서 사용되며, 플레이어 입력 → 이동 벡터 → 충돌 보정 → 최종 위치의 단순한 흐름으로 동작합니다.

### 도입 이유

Unity 물리 엔진으로부터 독립적이기에 물리적 연산이 필요한 기능은 따로 작성해야 하지만 직접 작성하여 예측 가능하며 제어하기 쉽다는 점에서 실제로 프로젝트에서 중력 관련 연산만 따로 필요하기에 굳이 Rigidbody를 사용하지 않은 CharacterController를 사용하여 캐릭터 움직임을 구현하였다.

### 주요 사용 포인트

| 사용한 메소드 | 사용한 기능 |
|---------------------------------------------------------------------------------------------------------------------------------------|---------------------|
| [Move](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/States/BaseState.cs) | Character Controller에 있는 velocity 프로퍼티를 사용하여 속도를 가져와 캐릭터가 움직일 수 있는 최대 속도지정 및 관성을 표현. |
| [Crouch_Async](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/Core/PlayerCondition.cs) | 플레이어가 앉을 때 Character Controller에 내장된 Collider Height와 Center를 변경하기 위해 사용 |
| [Update](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/States/Air/AirState.cs) | 플레이어의 현재 y축 velocity를 받아 떨어지고 있는 상태인지 측정하기 위해 사용 |
