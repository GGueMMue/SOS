using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LookAtController : MonoBehaviour
{
    public void PlayerRotate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane site = new Plane(Vector3.up, Vector3.up); // normal과 포인트가 Vector Up 방향인 무한대 Plane 생성

        if (site.Raycast(ray, out float dis)) // 만약 레이케스트가 맞았다면
        {
            Vector3 dir = ray.GetPoint(dis) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)); // 회전
        }
    }
}
