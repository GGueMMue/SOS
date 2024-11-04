SOS
-------------------------
제작 기간 1달 : (24-10-07 ~ 24-11-04)
탑 다운 슈팅 게임 개인 프로젝트입니다.

---------

개발환경
---------
Unity, C#

---------

플레이어블 플랫폼
---------
Windows

---------

플레이어 목표
---------
[최소 목표]적을 사살하고 클리어 포인트로 향할 것.

[확장 목표]최대한 고득점을 하여 게임을 클리어할 것.

---------

조작키
--------
|입력키|설명|
|------|---|
|W|플레이어 캐릭터의 전진 이동|
|S|플레이어 캐릭터의 후진 이동|
|A|플레이어 캐릭터의 좌측 이동|
|D|플레이어 캐릭터의 우측 이동|
|Mouse Location|해당 방향으로 캐릭터 회전 및 사격 방향 조절|
|Mouse Button 0|사격 또는 근접 공격|
|E|적 시체에 유저의 캐릭터가 가까이 있을 경우, 확인 사살|
|G|현재 무기 버리기, 또는 무기에 가까이 있을 경우 해당 무기로 교체|

--------

게임 흐름도
--------
```
 [시작 화면]  --   [Start 버튼]  ----------------------------> [스테이지 1 스타트] --------------------------|---- [클리어 시] -> [점수 표출 및, 다음 스테이지 시작] 
     ^  |-------- [continue 버튼] ->   [Json Data 불러오기] |-> [클리어 데이터 있음] -> 선택 스테이지 스타트]  -|---- [ToMain 버튼 클릭시]
     |  |                                                  |-> [클리어 데이터 없음]                                          |
     |  |________ [Exit 버튼] -> 게임 종료                              |                                                    |
        |______________________________________________________________|____________________________________________________|
```

시작화면
-------
![시작화면](https://github.com/user-attachments/assets/c1d4e768-6e79-440d-b338-40b2f00c4cfe)

[Start 버튼]을 눌렀을 경우, Stage 1번으로 넘어가 진행하게 됩니다.
[Exit 버튼]을 눌렀을 경우, 게임이 종료됩니다.

![1Con](https://github.com/user-attachments/assets/d594c2a1-8e8e-4ad6-aab5-e8643e1ccb79)

[Continue 버튼]을 눌렀을 경우 보이는 창
큰 패널 내 4개의 스테이지 상자는 그 자체로 버튼이며, Json에 클리어 기록이 있는 경우, 버튼이 활성화됩니다.

![점수표출화면](https://github.com/user-attachments/assets/59f132ac-46b3-4037-aa5b-158506dda13d)
점수 표출 화면입니다.
싱글톤으로 구현된 유저가 플레이하며 기록한 각 수치가 담긴 변수 만큼 총알이 생성되며 유저가 해당 플레이 판에서 어떤 기록을 이뤘는지 표출합니다.

이때, 클리어 기록을 Json 파일로 저장합니다.


플레이어 캐릭터의 애니메이터
--------
플레이어 캐릭터의 애니메이터는 레이어로 상체와 하체가 나눠져 있습니다.

![하체](https://github.com/user-attachments/assets/541be4b3-eeb9-4dcb-aa49-d34bffcb0b79)

하체의 애니메이터 레이어입니다.

![상체](https://github.com/user-attachments/assets/a2f30aef-31c9-41d1-a6dc-dfe516b94ad6)

상체의 애니메이터 레이어입니다.


하체의 애니메이션 state는 유저의 입력 방향의 벡터와 플레이어 캐릭터가 바라보고 있는 방향의 백터 각을 뽑아내어, 해당 값이 사전에 설정하여둔 수치에 맞는 애니메이션 노드가 결정됩니다.

적의 상태머신
--------

|상태명|설명|
|------|---|
|IDLE|기본적인 상태. 적 유닛의 자식 오브젝트로 달린 Find 오브젝트의 Trigger을 통해, 유저를 찾음. 찾았을 경우 FIND 상태로 전환|
|IDLE_Patrol|IDLE 상태이나, 순찰하는 유닛. 노드를 통해 구현하였음.|
|Find|적 유닛이 플레이어를 발견하였고, 추격하는 상태. 이때, 같은 적 유닛의 자식 오브젝트로 달린 Attack 오브젝트의 콜라이더가 Trigger 되었을 때, Attack으로 전환|
|ROAMER|적 유닛이 Find 상태에서 유저를 놓쳤고, 주워진 유예 시간 내에서도 유저를 찾지 못한 경우, 또는 근처에 적 유닛의 시체가 존재하고 있을 경우 전환되는 상태. 랜덤한 방향으로 회전하며 일정 시간 동안 흥분 상태에 빠짐.|
|Attack|플레이어를 향해 공격 진행|

상태머신 진행도

                        IDLE (OR IDLE_PATROL)      ㅡㅡ             ROAMER
                                  |                 X                 |
                                FIND               ㅡㅡ             ATTACK


---------

적의 추격 및 공격 조건
--------

![range](https://github.com/user-attachments/assets/564a408f-d3b7-4ec3-9f34-d754adb63afc)

[SMG 적의 판정 기준]
적 유닛은 파란색으로 마크 쳐진 Find 범위에 유저가 들어 섰을 때, 유저를 향해 Raycast를 쏩니다.

이때, 적 유닛의 Raycast의 hit이 user가 아닌 벽이라면, 정면이 아니니 계속해서 IDLE 상태를 유지합니다.

Attack 범위에 들어오게 되면, 적 유닛은 Find 오브젝트를 끄고, Attack 상태로 전환합니다.
Attack 범위에 다시 벗어나게 되면, Find로 전환합니다.


적의 사망
---------

![2314캡처](https://github.com/user-attachments/assets/3f3069d1-d300-4049-b738-d58de442b4fc)

적 유닛은 사망한 경우, 자신이 들고 있던 무기를 드랍하는 동시에, 유저가 확인사살을 진행하기 전까지
일정 범위 내 모든 적들의 상태를 ROAMER 상태로 만듭니다.

적의 공격
---------

![적사격](https://github.com/user-attachments/assets/9d404dce-a1e0-4b8e-958e-8ba658675b25)

적의 근접 공격은 근접 공격의 애니메이션이 끝났을 때 판정이 이행됩니다.

적의 총기 공격은 투사체 형식으로 이뤄져, 유저가 피할 수 있게끔, 이행되며, 샷건은 5발의 산탄을 동시에 발사합니다.

유저는 해당 공격에 단 한 번이라도 피격되면 사망합니다.


클리어 포인트 화살표
---------

![클리어포인트](https://github.com/user-attachments/assets/c3005778-61b8-401e-bff6-bc3c8d2e8d11)
[클리어포인트]

![Arrow](https://github.com/user-attachments/assets/36e062dc-9cce-4044-8e69-82ebbf2aed94)
[화살표 사진]

Start될 때, 모든 적들에 대한 정보를 List로 받아온 뒤, 유저가 이를 모두 죽이게 되면 해당 포인트와 포인트를 가리키는 화살표가 활성화됩니다.

해당 포인트는 라인렌더러로 그려진 오브젝트이며, 포인트를 가리키는 화살표는 해당 포인트가 유저의 화면에 보이게 되면 사라지게 됩니다.
