using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player_LookAtController : MonoBehaviour
{
    Vector3 dir;
    public void PlayerRotate() // 유저 캐릭터의 마우스 방향에 따른 회전 함수
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane site = new Plane(Vector3.up, Vector3.up); // normal과 포인트가 Vector Up 방향인 무한대 Plane 생성

        if (site.Raycast(ray, out float dis)) // 만약 레이케스트가 맞았다면
        {
            dir = ray.GetPoint(dis) - transform.position;

            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)); // 회전
        }
    }

    public Vector3 ReturnNormalDir()
    {
        return new Vector3(dir.normalized.x, 0, dir.normalized.y);
    }
}
