# 👩‍💻 Player

<br>

![img_2.png](https://github.com/Neronem/TheLastOne_Public/blob/main/Game%20Client%20Architecture/Images/img_2.png)

| 클래스 | 설명 |
|--------|------|
| Player | 플레이어 관련 모든 클래스를 초기화시키고 관리합니다. |
| PlayerInventory | 소유한 아이템 목록을 관리합니다. |
| PlayerRecoil | 총기 반동 관련 로직을 관리합니다. |
| PlayerGravity | 플레이어의 중력을 관리합니다. |
| PlayerInput | 플레이어 인풋시스템을 외부에서 참조할 수 있도록 합니다. |
| PlayerInteraction | 플레이어의 상호작용을 관리합니다. |
| PlayerWeapon | 보유 중인지 여부, 개조 효과 등 무기 시스템을 관리합니다. |
| PlayerCondition | 플레이어의 스탯과 상호작용하는 모든 로직을 다룹니다. |

<br>

---

<br>

![img_3.png](https://github.com/Neronem/TheLastOne_Public/blob/main/Game%20Client%20Architecture/Images/img_3.png)

| 클래스 | 설명 |
|--------|------|
| PlayerStateMachine | 상태머신에 필요한 모든 정보를 가져옵니다. |
| BaseState | 플레이어 입력과 같은 기초 상태를 정의합니다. |
| GroundState | 플레이어가 지면에 있을 시의 공통로직을 정의합니다. |
| AirState | 플레이어가 공중에 있을 시의 공통로직을 정의합니다. |
| 이외 | AirState 또는 GroundState를 상속받아 상태를 구현합니다. |