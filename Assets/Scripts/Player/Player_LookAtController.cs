using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player_LookAtController : MonoBehaviour
{
    Vector3 dir;
    public void PlayerRotate() // ���� ĳ������ ���콺 ���⿡ ���� ȸ�� �Լ�
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane site = new Plane(Vector3.up, Vector3.up); // normal�� ����Ʈ�� Vector Up ������ ���Ѵ� Plane ����

        if (site.Raycast(ray, out float dis)) // ���� �����ɽ�Ʈ�� �¾Ҵٸ�
        {
            dir = ray.GetPoint(dis) - transform.position;

            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)); // ȸ��
        }
    }

    public Vector3 ReturnNormalDir()
    {
        return new Vector3(dir.normalized.x, 0, dir.normalized.y);
    }
}
