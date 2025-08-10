# 🛠️ 적 AI (Enemy AI)

## 목차

- [🌙 OverView 🌙](#overview)
- [🔄 Behavior Tree 🔄](#bt)
- [⚙️ StatController ⚙️️️](#stat)
---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 적 AI를 구현하기 위해 적용된 기술 스택을 소개합니다.

<br>

---

<a name="bt"></a>
## 🔄 Behavior Tree

### 개념

![img.png](img.png)

Behavior Tree는 AI 의사결정을 트리 구조로 계층화하여 관리하는 설계 기법입니다.  
각 노드는 조건 검사나 행동 수행 같은 역할을 담당하며, 트리 구조를 통해 복잡한 의사결정을 모듈화하고 체계적으로 처리할 수 있습니다.

### 도입 이유

- 복잡한 행동 패턴의 모듈화 및 재사용성 향상
- 조건 재평가 주기와 Running 상태 제어를 명확히 하여 자연스러운 행동 전환 구현
- 시각적 툴(Behavior Designer)로 디버깅과 트리 관리 용이

### 주요 메서드 및 기능

| 메서드                                                                                                                                                                 | 기능                                 |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------|
| [OnStart](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/AIBehaviors/BehaviorDesigner/Action/ShebotOnly/ShebotRifleFire.cs#L25)  | 노드가 처음 활성화될 때 한 번 호출되며 초기화 작업이나 준비 동작을 처리합니다. |
| [OnUpdate](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/AIBehaviors/BehaviorDesigner/Action/ShebotOnly/ShebotRifleFire.cs#L39) | 노드가 활성 상태인 동안 매 프레임 호출되어 실제 행동 수행과 상태 반환을 담당합니다. |

<br>

---

<a name="stat"></a>
## ⚙️ StatController 

### 개념

StatController는 AI 개체들의 스탯 데이터와 변경 로직을 관리합니다. <br>
각 AI별 특성에 맞는 스탯 정보를 제공하고, 변경사항을 반영합니다.

### 도입 이유
- NPC마다 필요한 능력과 스탯 항목이 다양하지만, 모든 스탯을 전역 필드로 선언하면 메모리 낭비 발생 가능성 有
- 스탯 구조를 유연하고 확장성 있게 관리할 필요성 대두
- 스탯 변경 로직이 대부분 Npc에 중복되는 경우 다수

### 주요 메서드 및 기능

| 메서드                                                                                                                                                                | 기능                                                                   |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------|
| [TryGetRuntimeStatInterface](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/StatControllers/Base/BaseNpcStatController.cs#L383) | 현재 AI가 보유중인 스탯이 인터페이스를 포함하는지의 여부를 체크합니다.<br/> 포함 시엔 해당 인터페이스를 반환합니다. |
| [OnTakeDamage](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/StatControllers/Base/BaseNpcStatController.cs#L164)               | 스탯 복사본의 체력값에 현재 방어력을 연산하여 데미지를 입힙니다.                                 |
| [OnBleed](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/StatControllers/Base/BaseNpcStatController.cs#L210)                    | 스탯 복사본의 체력값에 출혈데미지를 연산하여 데미지를 입힙니다.                                  |
| [OnStunned](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/StatControllers/Base/BaseNpcStatController.cs#L317)       | isStunned 플래그를 관리하고 AI 상태를 리셋시킵니다.                                   |
| [Hacking](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Entity/Scripts/NPC/StatControllers/Base/BaseNpcStatController.cs#L228C13-L229C42)         | 스탯의 isAlly값을 변경하고 레이어를 교체합니다.                                        |

<br>

----