# 🔧 TimeScale 변형 시 캐릭터 속도 증가 버그

## 목차

- [🌙 OverView 🌙](#overview)
- [⚠️ 문제 현상 ⚠️](#problem)
- [🔍 원인 분석 🔍](#search)
- [💡 해결 방법 💡](#solve)

---

<br>

<a name="overview"></a>
## 🌙 OverView

현 페이지에선 Focus 시스템을 구현하던 중 생긴 문제인 <br>
`TimeScale 변형 시 캐릭터 속도 증가 버그`을 설명합니다.

<br>

---

<br>

<a name="problem"></a>
## ⚠️ 문제 현상
- TimeScale을 변경하면, Character Controller 컴포넌트 자체의 velocity가 무한대로 빨라짐

<br>

---

<br>

<a name="search"></a>
## 🔍 원인 분석

![img_2.png](img_2.png) <br>
<sub>Time.DeltaTime은 Time.unscaledTime * Time.timeScale입니다.</sub>

- velocity의 내부 연산 과정 = (프레임 간 이동한 거리 / Time.DeltaTime)이므로, <br>
DeltaTime이 갑자기 변경되면 속도에 큰 영향이 가며 무한대로 변함

<br>

---

<br>

<a name="solve"></a>
## 💡 해결 방법

![img_3.png](img_3.png) <br>
<sub>Time.timeScale을 곱함으로써 상쇄</sub>

- 속도를 결정하는 코드에서 Time.timeScale를 곱함으로써 원래속도로 보정하게 함

<br>

---
