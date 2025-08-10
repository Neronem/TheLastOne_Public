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

이 표를 복붙하면서 작성해주세요

| 메서드                                                                                                                                   | 기능                  |
|---------------------------------------------------------------------------------------------------------------------------------------|---------------------|
| [PlayerStateMachine](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/Player/StateMachineScripts/PlayerStateMachine.cs#L30) | 이 메서드는 무슨 역할을 합니다.. |
| 쭉 예시 : Initialize                                                                                                                     | FSM 초기화 및 상태 설정을 담당합니다. |
| ChangeState                                                                                                                           | 현재 상태를 다른 상태로 전환하는 기능을 수행합니다. |
| UpdateState                                                                                                                           | 매 프레임마다 현재 상태에 따른 동작을 실행합니다. |
| ExitState                                                                                                                             | 상태 종료 시 필요한 정리 작업을 처리합니다. |

<br>

---

<br>

<a name="character-controller"></a>
## 🎮 Character Controller

개념 :
<br>
도입 이유 :
<br>
주요 메서드 및 기능 :

<br>

---