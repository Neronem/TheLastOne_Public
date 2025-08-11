# 🗣️ 대사 시스템 (Dialogue System)

## 목차

* [🌙 OverView 🌙](#-overview-)
* [🧠 데이터 모델 (DialogueData/DialogueDataSO)](#-데이터-모델-dialoguedatadialoguedataso)
* [🧭 런타임 플로우 (Manager ↔ UI)](#-런타임-플로우-manager--ui)
* [🎯 트리거 & 직접 호출 예시](#-트리거--직접-호출-예시)
* [🛠️ 주요 메서드 및 기능](#-주요-메서드-및-기능)
* [🌐 Localization 연동](#-localization-연동)

---

<br>

## 🌙 OverView 🌙

이 시스템은 \*\*데이터(대사 시퀀스)–런타임(매니저)–표현(UI)\*\*를 분리한 3계층 구조입니다.

* `DialogueDataSO`에 **키 기반 시퀀스**를 저장하고,
* `DialogueManager`가 **저장/중복 재생 방지/캐싱**을 책임지며,
* `DialogueUI`가 **타자기 효과·보이스 재생·페이드** 등 연출을 담당합니다.

---

<br>

## 🧠 데이터 모델 (DialogueData/DialogueDataSO)

* **DialogueData**: 화자명, 본문(LocalizedString), 화자 타입, 보이스(LocalizedAsset<AudioClip>)로 구성된 1줄 단위 구조체. **언어와 음성**을 테이블로부터 지연 로딩합니다.
* **DialogueDataSO**: `dialogueKey`(정수)와 `List<DialogueData> sequence`로 하나의 대화 시퀀스를 묶어 관리합니다. (Addressables로 수집/캐시)

---

<br>

## 🧭 런타임 플로우 (Manager ↔ UI)

1. **캐싱/세이브 체크**
   `DialogueManager.CacheDialogueData()`가 Addressables에서 모든 `DialogueDataSO`를 읽어 \*\*dict\[dialogueKey]\*\*로 캐싱합니다.
   재생 여부는 `BaseDialogueIndex + key`를 **씬별 completionDict**로 관리하여 **한 번만** 재생할 이벤트를 막아줍니다.

2. **재생 트리거**
   `TriggerDialogue(key)`가 UIManager에서 `DialogueUI`를 가져와 `ShowSequence(sequence)`를 호출합니다. **일시정지 중에는 무시**하도록 방어합니다.

3. **표현(타자기/보이스/페이드)**
   `DialogueUI.ShowSequence()` → 내부 코루틴 `PlayDialogueSequence()`가

* `LocalizedString.StringChanged` / `LocalizedAsset.AssetChanged`로 **문장·보이스를 비동기 확보**,
* 화자 타입에 따라 **색/프레임 테마** 전환,
* `SoundPlayer`(풀에서 꺼냄)로 **보이스 2D 재생**,
* 글자마다 **TypeWriter SFX**와 함께 한 글자씩 출력,
* 보이스 길이(없으면 1.5초) 대기 후 **역타자(삭제) 효과**로 마무리,
* 다음 인덱스로 재귀 진행합니다. 모든 타이밍은 **Realtime(일시정지 무시)** 기반입니다.

4. **수명주기/정리**
   `Hide()/ResetUI()`는 코루틴/보이스를 정지하고 캔버스를 페이드아웃한 뒤 비활성화하여 **누수/중복 재생**을 방지합니다.

---

<br>

## 🎯 트리거 & 직접 호출 예시

* **콜라이더 트리거 기반**: `DialogueTrigger`가 `OnTriggerEnter`에서 Player 태그를 검사하고, 이미 재생된 키는 무시. 재생되면 `MarkAsPlayed`까지 처리하여 **중복 호출**을 차단합니다.
* **스크립트 직접 호출(예: BaseNPCStatController)**: 적 **첫 처치** 시 `DialogueManager.TriggerDialogue(FirstKillDialogueKey)`를 직접 호출하여 대사를 띄웁니다(전역 1회).

---

<br>

## 🛠️ 주요 메서드 및 기능


| 메서드/타입                                                         | 기능                                                                  |
| -------------------------------------------------------------- | ------------------------------------------------------------------- |
| `DialogueManager.CacheDialogueData()`                          | Addressables에서 `DialogueDataSO`를 수집해 `dict[key]` 캐싱.                |
| `DialogueManager.HasPlayed(int key)` / `MarkAsPlayed(int key)` | 씬별 세이브 딕셔너리로 재생 여부 확인/기록. “1회성 대사” 보장.                              |
| `DialogueManager.TriggerDialogue(int key)`                     | UIManager로부터 `DialogueUI`를 얻어 `ShowSequence` 호출(일시정지 시 무시).         |
| `DialogueUI.ShowSequence(List<DialogueData>)`                  | 시퀀스 표시 진입점. 내부 코루틴 시작 및 페이드인 초기화.                                   |
| `DialogueUI.PlayDialogueSequence(...)`                         | (코루틴) Localized 텍스트/보이스 확보 → 테마 스킨 → 보이스 재생 → 타자기/역타자 연출 → 다음 줄 재귀. |
| `DialogueUI.Hide()/ResetUI()`                                  | 코루틴/보이스 정지, 캔버스 알파/비활성화로 정리.                                        |
| `DialogueTrigger.OnTriggerEnter(Collider)`                     | 플레이어 진입 시 키 검사 → 미재생이면 Trigger + MarkAsPlayed.                      |
| `직접 호출 (예:BaseNpcStatController)`                                   | 해당 조건에서 직접 `TriggerDialogue(DialogueKey)` 호출(전역 1회).                    |

---

<br>

## 🌐 Localization 연동

* **문장(Localization String Table)**: `LocalizedString.StringChanged`를 구독해 **문장 로딩 완료 시점**에 출력 시작(타자기 효과).
* **보이스(Asset Table)**: `LocalizedAsset<AudioClip>.AssetChanged`에서 **보이스 클립**을 받아, 풀에서 꺼낸 `SoundPlayer`로 2D 재생합니다. (길이로 대기시간 산정)
* **스킨(화자 타입)**: Player/Ally/Enemy에 따라 텍스트/프레임의 컬러 타입을 변경하여 **시각적 발화자 구분**을 제공합니다.
* 모든 대기·페이드·타자기는 `WaitForSecondsRealtime` / `unscaledDeltaTime` 기반이어서 **일시정지에도 흐름이 깨지지 않습니다.**
* **성능/안정성**: 보이스는 **풀링된 SoundPlayer**로 재생하고, Hide/Reset에서 정리 → 장시간 플레이에도 누수 방지.
* **다국어 제작 효율**: 텍스트/보이스를 Localization 테이블로 분리하여 **새 언어 추가 시 코드 수정 없음**.
