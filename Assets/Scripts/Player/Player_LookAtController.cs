using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LookAtController : MonoBehaviour
{
    public void PlayerRotate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane site = new Plane(Vector3.up, Vector3.up); // normal�� ����Ʈ�� Vector Up ������ ���Ѵ� Plane ����

        if (site.Raycast(ray, out float dis)) // ���� �����ɽ�Ʈ�� �¾Ҵٸ�
        {
            Vector3 dir = ray.GetPoint(dis) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)); // ȸ��
        }
    }
}
