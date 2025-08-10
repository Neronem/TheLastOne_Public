# 🛠️ 게임 이벤트 시스템 (Game Event System)

## 목차

- [🌙 OverView 🌙](#overview)
- [📢 GameEventSystem 📢](#GameEventSystem)
---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 게임 내 이벤트를 통합 관리하는데 사용한 기술을 소개합니다.

<br>

---

<br>

<a name="GameEventSystem"></a>
## 📢 GameEventSystem

### 개념
게임 내에서 발생할 수많은 이벤트 종류들을 한 곳에서 관리하여 유지보수성을 높이는 시스템입니다. <br>
이벤트가 필요한 곳에서 RegisterListener()를 실행하고, <br>
특정 이벤트가 일어날 시 등록된 IGameEventListener 전체를 순회하며 알맞는 인덱스의 카운트를 올립니다.

### 도입 이유
- 이벤트 발행자와 수신자가 서로 직접 참조하지 않고도 데이터 전달 가능
- 새로운 이벤트나 리스너 추가 시 기존 코드 수정 최소화
- 이벤트 흐름을 한 위치에서 관리하여 디버깅과 유지보수가 쉬움
- 필요한 리스너만 등록하여 조건부로 이벤트 처리 가능
- 동일한 이벤트를 다양한 시스템(UI, 퀘스트, 오브젝트 제어 등)에 동시에 활용 가능

### 주요 메서드 및 기능

| 메서드                                                                                                                                                                | 기능                                                       |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------|
| [RegisterListener](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Quests/Core/GameEventSystem.cs#L20)  | IGameEventListener를 가진 객체가 자신을 이벤트시스템에 등록합니다.            |
| [UnregisterListener](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Quests/Core/GameEventSystem.cs#L26) | IGameEventListener를 가진 객체가 자신을 이벤트시스템에서 해제시킵니다.          |
| [RaiseEvent](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Quests/Core/GameEventSystem.cs#L32) | 등록된 IGameEventListener 전체를 순회하며 알맞는 인덱스의 이벤트 카운트를 추가합니다. |

<br>

----
