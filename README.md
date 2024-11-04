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

                        IDLE (OR IDLE_PATROL)      ㅡ             ROAMER
                                  |                 X                |
                                FIND               ㅡ             ATTACK

---------



