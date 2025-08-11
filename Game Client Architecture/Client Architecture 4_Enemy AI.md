# 👩‍💻 Enemy AI

<br>

![img_4.png](https://github.com/Neronem/TheLastOne_Public/blob/main/Game%20Client%20Architecture/Images/img_4.png)

| 클래스 | 설명 |
|--------|------|
| Task | BT 모든 노드의 부모 추상클래스입니다. 노드로써 기능할 기본 구조를 갖춥니다. |
| Decorator | 자식 노드 실행 조건을 제어하는 노드 (예: 조건 만족 시 실행, 반복 실행, 지연 실행) |
| Composite | 여러 자식 노드를 순서대로 또는 조건에 맞게 실행하는 노드 (예: Selector, Sequence) |
| Action | 실제 동작(액션)을 수행하는 리프 노드 (예: 걷기, 점프, 공격) |
| Condition | 특정 상태를 확인하고 성공/실패를 반환하는 리프 노드 (예: 지면에 있는가?, 체력이 50% 이하인가?) |

<br>

---

<br>

![img_5.png](https://github.com/Neronem/TheLastOne_Public/blob/main/Game%20Client%20Architecture/Images/img_5.png)

| 클래스 | 설명                                            |
|--------|-----------------------------------------------|
| BaseNpcStatContoller | Npc라면 가져야할 공통 스탯 변경 로직 (예: 해킹, 스턴, 출혈)을 갖춥니다. |
| 이외 | 자식들은 Base를 상속받아 각자만의 스탯을 선언합니다.       |