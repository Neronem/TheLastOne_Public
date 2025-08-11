# 👩‍💻 UI

<br>

* InGameUI 예시

![img_6.png](https://github.com/Neronem/TheLastOne_Public/blob/main/Game%20Client%20Architecture/Images/img_6.png)

| 클래스 | 설명 |
|--------|------|
| UI Manager | UI를 그룹 단위(로비/인게임/HUD/일시정지)로 등록·표시·숨김·리셋합니다. GetUI<T>()로 UI 간 중개를 담당합니다. |
| UI Base | 모든 화면이 따르는 수명주기 규약을 제공합니다: Initialize / Show / Hide / ResetUI. |
| 개별 UI | UIBase 규약(Initialize/Show/Hide/ResetUI)을 따르며 표시·연출·데이터 바인딩만 담당하고, UIManager(GetUI<T>())를 통해서만 상호작용하며 언어 전환/씬/일시정지 시 상태를 안전히 정리합니다. |

<br>

---