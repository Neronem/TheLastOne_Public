# 🌐 로컬라이제이션 (Localization)

## 목차

* [🌙 OverView 🌙](#-overview-)
* [🧠 데이터/테이블 구조](#-데이터테이블-구조)
* [🧭 런타임 플로우](#-런타임-플로우)
* [🧩 UI 연동 패턴(텍스트오디오)](#-ui-연동-패턴텍스트오디오)
* [🖋️ 폰트 전략](#-폰트-전략)
* [🛠️ 주요 메서드 및 기능](#-주요-메서드-및-기능)
* [✅ 장점](#장점)

---

<br>

## 🌙 OverView 🌙

프로젝트 전반(UI, 대화, 튜토리얼, 아이콘/로고 등)에 Unity **Localization 패키지**를 적용했습니다. 핵심은 다음과 같습니다.

* **String Table**: 모든 UI 텍스트를 키 기반으로 관리(한/영 등 다국어).
* **Asset Table**: **AudioClip**을 언어별로 분기.
* **런타임 전환**: 설정 UI에서 Locale 변경 시 **즉시 반영**, 선택 값은 **PlayerPrefs**로 저장/복원.
* **UIManager/Dialogue와 결합**: UIBase 수명주기(Initialize/Show/Hide/ResetUI)에 맞춰 바인딩/해제, Dialogue는 보이스를 **LocalizedAsset**으로 로드 후 **SoundPlayer**로 재생.

---

<br>

## 🧠 데이터/테이블 구조

* **String Table**
  버튼/라벨/토스트/팝업/튜토리얼 등 모든 UI 텍스트를 키로 등록. 긴 문장은 **Smart String**으로 변수 삽입 가능.

* **Asset Table**
  * **AudioClip**: 대사 보이스(언어별 음성), 내레이션.

* **Addressables 연동**
  Asset Table 항목은 Addressables와 연결되어 빌드/메모리 관리에 유리(라벨 기반 로드/언로드).

---

<br>

## 🧭 런타임 플로우

1. **부팅/초기화**
   `PlayerPrefs`에서 저장된 Locale을 읽어 `SelectedLocale` 적용 → 첫 진입부터 올바른 언어.

2. **설정 UI에서 언어 변경**
   `ApplyLocale(localeId)` 호출 → `LocalizationSettings.SelectedLocale` 갱신.
   모든 `LocalizedString/LocalizedAsset` 구독자들이 **StringChanged/AssetChanged** 이벤트 수신.

3. **세션 저장**
   선택한 언어를 PlayerPrefs에 저장 → 다음 실행 시 자동 복원.

---

<br>

## 🧩 UI 연동 패턴(텍스트/이미지/오디오)

* **텍스트(TMP)**
  각 컴포넌트에 `LocalizedString`을 바인딩하고 `StringChanged +=`로 값이 들어오는 시점에 `text` 갱신.
  토스트/알림은 UIBase의 `Show()` 타이밍과 맞춰 **지연 로딩 후 표시**.

* **오디오(대사 보이스)**
  `LocalizedAsset<AudioClip>`을 DialogueUI에서 확보 → **SoundPlayer(2D)** 로 재생.
  **클립 길이**를 이용해 타자기/페이드 타이밍 결정 → 언어마다 길이가 달라도 자연스럽게 동기화.

> UI는 **UIBase 수명주기(Initialize/Show/Hide/ResetUI)** 에 맞춰 바인딩을 추가/해제하여 코루틴·오디오 누수를 방지합니다.

---

<br>

## 🖋️ 폰트 전략

* **폰트(Fallback 체인)**
  한글/영어 폰트를 모두 커버하도록 **TMP Font Asset Fallback** 구성.
  언어가 추가되어도 같은 방식으로 구성하여 커버 가능.

---

<br>

## ✅ 장점

* **개발 효율**: 문자열/애셋을 **테이블로 분리** → 언어 추가 시 코드 수정 없이 데이터만 증분.
* **일관성**: 텍스트/보이스가 **같은 Locale**에 맞춰 **동시에 교체**.
* **UX 품질**: 보이스 길이에 맞춘 타자기·페이드.
* **배포/운영 비용 절감**: 하나의 빌드로 다국어 커버, 지역별 리소스 변경도 **Asset Table 교체**만으로 대응.
