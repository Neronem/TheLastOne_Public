# 🛠️ 매니저 시스템 (Manager System)

## 목차

- [🌙 OverView 🌙](#overview)
- [🗝️ CoreManager 🗝️](#CoreManager)
---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 매니저들의 전역적인 접근점을 제공하는 코어매니저 시스템에 대해 소개합니다.

<br>

---

<br>

<a name="CoreManager"></a>
## 🗝️ CoreManager

### 개념
CoreManager는 게임 내 여러 주요 시스템(매니저)들을 중앙에서 총괄하고 관리하는 싱글톤 클래스입니다. <br>
게임 실행 중 필요한 서브 매니저들을 초기화, 시작, 갱신, 종료하며, 시스템 간의 의존성을 조율합니다. <br>
또한 씬 전환, 저장/로드, 취소 토큰 관리 같은 공통 기능을 제공하여 게임 전체 흐름을 원활하게 유지합니다.

### 도입 이유
- 하위 매니저들의 초기화 순서 보장
- 서브 매니저들은 CoreManager를 통해 간접적으로 연결되므로 의존성 최소화
- 저장, 로딩, 씬 전환 등 비동기 작업을 중앙에서 안전하게 큐잉하고 관리 가능

### 주요 메서드 및 기능

| 메서드                                                                                                                    | 기능                                                      |
|------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------|
| [Awake](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Manager/Core/CoreManager.cs#L47)                | 모든 서브매니저를 메모리에 생성합니다.                                   |
| [Start](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Manager/Core/CoreManager.cs#L83)                | 각자 매니저들의 초기화를 순서대로 진행합니다.                               |
| [SaveData_QueuedAsync](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Manager/Core/CoreManager.cs#L133)        | 이전 저장 작업이 끝날 때까지 대기 후 저장 작업을 순차적으로 실행합니다.               |
| [LoadDataAndScene](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Manager/Core/CoreManager.cs#L149)        | 저장 데이터를 불러오고 해당하는 씬을 로드합니다.                             |
| [CreateNewCTS](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Manager/Core/CoreManager.cs#L121) | 모든 주요 비동기 작업용 취소 토큰(CancellationTokenSource)을 새로 생성합니다. |

<br>

----
