# 🌐 로컬라이제이션 (Localization)

## 목차

* [🌙 OverView 🌙](#-overview-)

* [🌐 로컬라이제이션 🌐](#localization)

---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 언어 2개 지원을 구현하기 위해 적용된 기술 스택, Localization에 대해 소개합니다.

<br>

---

<br>

<a name="localization"></a>
## 🌐 로컬라이제이션 

### 개념
- Localization 패키지는 게임 내 다국어 지원을 쉽게 구현하고 관리하기 위한 공식 툴킷입니다.
- 키 기반 관리: Dialogue_Stage1.AI_001/ Voice_Stage1.Pl_001 처럼 도메인·하위·키로 텍스트/애셋을 등록합니다.
- 이벤트 드리븐 갱신: LocalizedString.StringChanged, LocalizedAsset.AssetChanged 이벤트로 로딩 완료 시점에 UI를 갱신합니다.
- 런타임 스위칭: SelectedLocale 변경 시 전 UI가 즉시 교체, 선택 값은 PlayerPrefs로 저장/복원합니다.
- 레이아웃·폰트 전략: 전환 직후 길이 변화 대응을 위해 페이드 아웃 → 재활성 → 레이아웃 리빌드 → 페이드 인을 표준화하고, TMP Fallback 체인으로 글리프 누락을 방지합니다.

### 도입 이유
- 커스텀 대비 개발/운영 비용 절감(툴링, 이벤트, 테이블 관리가 내장)
- 코드 수정 없이 언어 증설 가능(데이터 드리븐)
- 전환 시 즉시성·안정성(이벤트 기반 업데이트 + 레이아웃 리빌드 루틴)
- 문자열뿐만 아니라 보이스 클립까지 전 리소스 계층을 일괄 관리

<img width="1631" height="1080" alt="image" src="https://github.com/user-attachments/assets/223fd326-af74-456f-a119-3119a24058f8" />
<img width="1631" height="689" alt="image" src="https://github.com/user-attachments/assets/4db520a1-aba2-41f0-af3a-ffdc67c1ef40" />

### 🛠️ 주요 메서드 및 기능

| 메서드/항목                                                                                                                                             | 기능                                                                                                                                                     |
|----------------------------------------------------------------------------------------------------------------------------------------------------| ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| [SettingUI.OnEnable()](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L46)                                 | 로케일 변경 이벤트를 구독하고, 언어 선택 UI 이벤트를 재바인딩한 뒤 현재 로케일과 동기화합니다.                                                                                                |
| [SettingUI.OnDisable()](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L53)                                | 로케일 변경 이벤트 구독을 해제합니다(누수 방지).                                                                                                                           |
| [SettingUI.OnLanguageSelectorChanged(int idx)](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L87)         | 선택된 인덱스를 `LocalizationSettings.SelectedLocale`에 반영하고 PlayerPrefs에 저장합니다. 런타임에서 즉시 언어가 전환됩니다.                                                   |
| [SettingUI.SyncLanguageSelector()](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L77)                     | 현재 **SelectedLocale**을 읽어 언어 선택 UI 인덱스를 강제 동기화합니다. (언어 전환 후 화면 즉시 반영)                                                                                  |
| [SettingUI.RegisterLanguageSelectorEvents()](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L63)           | HorizontalSelector의 각 아이템에 콜백을 등록해 인덱스 변경 시 `OnLanguageSelectorChanged`가 호출되도록 설정합니다.                                                                  |
| [SettingUI.OnLocaleChanged(Locale)](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/Setting/SettingUI.cs#L58)                    | 외부에서 로케일이 바뀐 경우에도 UI 인덱스를 재동기화합니다.                                                                                                                     |
| [DialogueData.Message / voiceClip](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/Dialogue/DialogueData.cs#L10)                    | 각 대사 줄은 `LocalizedString`(자막)과 `LocalizedAsset<AudioClip>`(보이스)로 구성되어 언어별로 자동 교체됩니다.                                                                   |
| [DialogueUI.ShowSequence(List<DialogueData>)](https://github.com/Neronem/TheLastOne_Public/blob/main/Scripts/UI/InGame/Dialogue/DialogueUI.cs#L94) | 시퀀스 표시의 진입점. 페이드 준비 후 코루틴을 시작합니다. (일시정지 시 가드)                                                                                                          |
| `DialogueUI`(시퀀스 내부 로직)                                                                                                                            | 자막은 `LocalizedString` 값 로딩 완료 시 출력하고, 보이스는 `LocalizedAsset<AudioClip>` 로딩 후 2D 재생해 **언어별 길이**에 맞춰 대기·타자기 타이밍을 동기화합니다. (구조는 `ShowSequence`→코루틴 흐름에 포함)  |

<br>

---

