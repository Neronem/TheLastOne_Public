# 🛠️ 커스텀 디버그 윈도우 (Custom Debug Window)

## 목차

- [🌙 OverView 🌙](#overview)
- [🛠️ CustomDebugWindow 🛠️](#CustomDebugWindow)

---

<br>

<a name="overview"></a>
## 🌙 OverView

이 페이지는 유니티 에디터 환경에서 동작하는 커스텀 디버그 윈도우를 소개합니다.  
이 윈도우는 게임 개발 중 디버깅 편의를 위해 플레이어 상태 조작, 스폰 지점 생성, 무기 조작, UI 표시 등 다양한 기능을 버튼 클릭으로 쉽게 실행할 수 있도록 도와줍니다.

<br>

---


<br>

<a name="CustomDebugWindow"></a>
## 🛠️ CustomDebugWindow

### 개념
Unity Editor에서만 사용 가능한 디버그 전용 윈도우로, 개발자가 편하게 게임 상태를 조작하고 테스트할 수 있는 기능들을 제공합니다.  
게임 플레이 중 특정 상태 회복, 아이템 및 적 스폰 위치 자동 수집과 ScriptableObject 생성, UI 표시 및 무기 관련 기능을 빠르게 테스트 해볼 수 있습니다.

### 도입 이유
- 빠른 테스트 및 디버깅 환경 구축


### 주요 버튼 및 기능

| 버튼명                                                                                                                              | 기능                                                  |
|----------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|
| [Recover Focus Gauge](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L36)             | 플레이어의 포커스 게이지를 디버그 목적으로 즉시 회복시킴                     |
| [Recover Instinct Gauge](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L43)          | 플레이어의 본능 게이지를 디버그 목적으로 즉시 회복시킴                      |
| [Find Spawn Points & Create S.O.](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L50) | 씬 내 아이템, 무기, 적 스폰 지점을 자동으로 찾아 ScriptableObject 형태로 저장 |
| [Get All Available Parts](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L122)        | 플레이어 무기의 모든 부품 획득                                   |
| [Forge Weapon](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L130)                   | 플레이어 무기 강화                                          |
| [Clear PlayerPrefs](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L138)              | 저장된 플레이어 환경 설정 초기화 및 저장                             |
| [Show ModificationUI](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L145)            | 커스텀 무기 개조 UI를 화면에 표시                                |
| [Hide ModificationUI](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L151)            | 커스텀 무기 개조 UI를 화면에서 숨김                               |
| [Show Ending Credit](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L157)             | 엔딩 크레딧 UI 표시                                        |
| [Show BleedOverlay](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L162)              | 플레이어 출혈 상태 효과 시연                                    |
| [FocusMode On](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L167)                   | 포커스 모드 활성화                                 |
| [FocusMode Off](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Debug/CustomDebugWindow.cs#L173)                  | 포커스 모드 비활성화                                |

<br>

---
