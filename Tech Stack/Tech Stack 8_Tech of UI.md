# 🧭 UI 매니저 (UIManager)

## 목차

* [🌙 OverView 🌙](#-overview-)
* [🧱 구조/그룹 설계](#-구조그룹-설계)
* [🧩 개별 UI(주요 화면) 목록](#-개별-ui주요-화면-목록)
* [🛠️ 주요 메서드 및 기능](#-주요-메서드-및-기능)
* [✅ 장점](#장점)
* [🧩 UIBase 기반 관리 심화](#-uibase-기반-관리-심화)
* [🔎 개별 UI 사례(How UIBase is used)](#-개별-ui-사례how-uibase-is-used)

---

<br>

## 🌙 OverView 🌙

본 페이지에선 **UIManager와 개별 UI 화면들**을 어떻게 구조화·운영했는지 소개합니다.
핵심은 \*\*“그룹 기반 동적 등록 + 제네릭 호출”\*\*입니다. 런타임에 필요한 UI만 생성/표시하고, 씬/상태 전환(예: 컷신, 일시정지, 로비↔인게임)에 맞춰 **일괄 Show/Hide/Reset** 합니다.

---

<br>

## 🧱 구조/그룹 설계

* **그룹 단위 관리**

  * `UIType`으로 **InGame\_HUD / InGame / Lobby / Pause** 등 그룹을 정의.
  * 그룹별 타입 목록을 보유하고, **제너릭/리플렉션**으로 일괄 호출(등록/표시/숨김/리셋).

* **정적 & 동적 등록**

  * **정적 등록**: `LoadingUI`, `LobbyUI`, `FadeUI` 등 **항상 필요한 UI**는 씬에 배치 → 시작 시 `RegisterStaticUI<T>()`.
  * **동적 등록**: 인게임 진입 등 **상황별 UI**는 Addressables에서 프리팹을 가져와 생성 → `RegisterDynamicUI<T>()` / `RegisterDynamicUIByGroup(UIType)`.

* **컷신/일시정지**

  * 컷신 시작 시 `InGame` 그룹 UI를 숨기고, 종료 시 복구.
  * 일시정지 시 `PauseMenuUI`만 노출되도록 HUD 일괄 Hide.

---

<br>

## 🧩 개별 UI(주요 화면) 목록

| 화면                      | 역할               | 표시 타이밍 / 트리거                |
| ----------------------- | ---------------- | --------------------------- |
| `LoadingUI`             | 리소스/씬 로딩 진행률 표시  | 씬 전환 직전\~완료 직후              |
| `LobbyUI`               | 메인 메뉴, 설정 진입     | 게임 시작 직후, 타이틀 상태            |
| `FadeUI`                | 페이드 인/아웃 트랜지션    | 씬 전환, 컷신/연출 전환              |
| `HUD`(체력/탄약/상호작용 등)     | 전투 중 실시간 정보      | 인게임 진입 시 `ShowHUD()`        |
| `DialogueUI`            | 대사/자막, 보이스 연동    | 이벤트/트리거 발생 시                |
| `MissionUI` / `QuestUI` | 진행률/목표 안내, 확장 패널 | 웨이브/퀘스트 갱신 시                |
| `InventoryUI`           | 장비/아이템 표시        |  `Tab` 입력 시                  |
| `ModificationUI`        | 무기 개조/부품 장착      | 워크벤치/특정 지점                  |
| `PauseMenuUI`           | 게임 정지/옵션         | `Esc` 입력, `ShowPauseMenu()` |

> 개별 UI는 공통 베이스 `UIBase`를 통해 `Initialize/Show/Hide/ResetUI` 인터페이스를 맞추고, UIManager가 한 줄로 제어합니다.

---

<br>

## 🛠️ 주요 메서드 및 기능


| 메서드                                               | 기능                                                                                                                |
| ------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| `Start()`                                         | `Canvas` 루트 확보, 정적 UI 등록(`RegisterStaticUI<LoadingUI/LobbyUI/FadeUI>()`), 초기 표시(`ShowUI<LobbyUI>()`, `FadeIn()`). |
| `RegisterStaticUI<T>()`                           | 씬에 미리 배치된 UI 컴포넌트를 찾아 `uiMap`에 등록하고 초기화.                                                                          |
| `RegisterDynamicUI<T>()`                          | Addressables에서 `typeof(T).Name`과 동일한 프리팹 로드 → 인스턴스화 → `uiMap` 등록/초기화. 이미 있으면 재초기화만 수행.                            |
| `RegisterDynamicUIByGroup(UIType)`                | 그룹에 속한 모든 UI 타입에 대해 위 **동적 등록**을 일괄 수행.                                                                           |
| `UnregisterDynamicUI<T>()`                        | `uiMap`에서 제거하고 GameObject 파기(동적 UI 수명 정리).                                                                        |
| `UnregisterDynamicUIByGroup(UIType)`              | 그룹에 속한 모든 동적 UI를 일괄 언레지스터/파기.                                                                                     |
| `GetUI<T>()`                                      | `uiMap`에서 타입 기반으로 즉시 조회.                                                                                          |
| `ShowUI<T>() / HideUI<T>()`                       | 개별 UI를 표시/숨김.                                                                                                     |
| `ShowHUD() / HideHUD()`                           | HUD 그룹 전체를 일괄 표시/숨김.                                                                                              |
| `ShowPauseMenu() / HidePauseMenu()`               | 일시정지 화면 게이팅 및 핸들러 연결.                                                                                             |
| `ResetHUD() / ResetUIByGroup(UIType)`             | HUD 또는 특정 그룹의 UI 상태를 일괄 초기화.                                                                                      |
| `OnCutsceneStarted(...) / OnCutsceneStopped(...)` | 컷신 중 InGame UI 숨김, 종료 시 복원.                                                                                       |

---

<br>

## ✅ 장점

* **메모리/성능**: 필요한 UI만 동적 생성·보관 → 초기 로딩/씬 전환 스파이크 감소, 메모리 사용량 절감.
* **안정성**: 컷신/일시정지/로딩처럼 **상태 전환 규칙**을 중앙에서 보장.
* **확장성**: 신규 UI 추가 시 **프리팹 + 타입 등록**만으로 플로우 합류(호출부 수정 최소화).
* **테스트 용이**: `GetUI<T>()`로 특정 UI만 독립 테스트 가능, 그룹 리셋으로 재현성↑.

---

<br>

## 🧩 UIBase 기반 관리 심화

### 개념

* 모든 화면이 **같은 생명주기 콜백**(Initialize/Show/Hide/ResetUI)을 가짐 → UIManager는 타입 제너릭으로 일괄 제어.
* `Initialize(UIManager, object param)`에서 매니저 참조 주입 및 1회 초기 상태 세팅.
* `Show/Hide`는 **가시성 제어의 단일 진입점**, `ResetUI`는 **상태 초기화**의 단일 진입점.

### 주요 콜백/계약 (복붙용 표)

| 메서드                                                  | 기능                                                  |
| ---------------------------------------------------- | --------------------------------------------------- |
| `Initialize(UIManager manager, object param = null)` | 매니저 참조를 주입하고(필수), 초기 상태를 준비합니다. (예: 캐시/이벤트 구독/비활성화) |
| `Show()`                                             | 화면을 보이게 하며, 필요한 코루틴/애니메이션을 **시작**합니다.               |
| `Hide()`                                             | 화면을 숨기고, 실행 중인 효과/코루틴을 **정리**합니다. (페이드아웃·음성중지 등)    |
| `ResetUI()`                                          | 데이터/게이지/텍스트/하이라이트 등 **모든 런타임 상태를 원점**으로 되돌립니다.      |

> 포인트: “표시 상태는 Show/Hide, 내용 상태는 ResetUI”로 분리되어 있어, **씬 전환/일시정지/컷신** 같은 이벤트에서 UI를 **안전하게 재초기화**할 수 있습니다.

---

<br>

## 🔎 개별 UI 사례(How UIBase is used)

### 1) InGameUI (HUD 패널)

* **Initialize**에서 시작 시 비활성화하고, 표시될 때 필요한 코루틴을 조건부 시작. (예: 게이지 100% 시 발광 효과 루프)
* **Show/Hide**는 코루틴의 **생명주기 경계**로 사용(표시 시 시작, 숨김 시 정지)하여 누수 방지.
* **ResetUI**는 체력 세그먼트 오브젝트 파기, 텍스트/슬라이더/게이지 초기화, 진행바/토스트 숨김까지 **완전 초기화** 수행.

핵심 포인트:

* **세그먼트 체력바**를 런타임 생성/보관(리스트) 후 Reset에서 완전 파기 → **씬 리로드 없이**도 안전 재사용.
* **LocalizedString**으로 토스트 문구를 비동기 로드해 표시/자동 숨김. (UIBase + Localization 결합 예)

---

### 2) DialogueUI (자막/보이스)

* **Show**에서 캔버스 알파 0으로 세팅 후 활성화 → 즉시 **페이드인 가능 상태**로 진입.
* **Hide/ResetUI**에서 **음성 중지**, 텍스트 정리, 페이드 코루틴 종료 → 다음 시퀀스 시작 시 꼬임 방지.
* **시퀀싱**: `ShowSequence(List<DialogueData>)`가 문자열/보이스를 **로컬라이즈** 비동기 로드 후, **타자기 효과** + **보이스 길이만큼 대기** → 순환 재생. (Realtime 기반)

핵심 포인트:

* 적/아군/플레이어 \*\*스킨(색/프레임)\*\*을 데이터의 SpeakerType으로 스위칭.
* **오브젝트 풀**의 `SoundPlayer`를 꺼내 2D 재생 후 자동 반환 → 오디오도 풀 기반으로 일관 관리.

---

### 3) ModificationUI (무기 개조)

* **Show/Hide**에서 **플레이어 이동/커서 상태**를 토글(잠금/해제)하여 UI 포커싱을 보장.
* **Initialize** 단계에서 프리뷰 맵/슬롯 이벤트 바인딩, Addressables에서 파트 데이터를 캐시.
* 파트 버튼을 선택하면 **하이라이트 머티리얼 교체**, 통계 슬라이더/텍스트 **프리뷰 업데이트**, Localized 텍스트로 이름/설명 표시.
* **ResetUI**로 모든 프리뷰/선택 상태/모달/텍스트를 초기화하여 **재진입 안전성** 확보.

보조 컴포넌트:

* **PartButton**은 메인/부가 버튼(장착/해제)을 **슬라이드 인/아웃** 애니메이션으로 노출(엔진 시간 영향X, `unscaledDeltaTime`). ModificationUI는 **상태만 알리면** 버튼은 **자체적으로 연출**을 처리.

---

### 4) HackingProgressUI (월드-고정 진행바, 예외 케이스)

* 이 UI는 `UIBase` 파생이 아니지만, **수명주기 규약을 내부에 재현**(SetTarget/Success/Fail → 애니메이션 → 풀 반환).
* **LateUpdate**에서 타겟 머리 위 오프셋 + 방향 고정. 성공/실패/취소 시 **오브젝트 풀로 Release**. → **UIBase 외부에서도 동일한 생명주기** 적용.

