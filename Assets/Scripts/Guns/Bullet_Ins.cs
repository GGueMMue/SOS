using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ins : MonoBehaviour
{
    public GameObject bullet;
    public Transform shotRocation;

    public void ShotBulletIns() // �� ���� ����. ������ �������̽�Ʈ ���
    {
        GameObject bullet_ = Instantiate(bullet);
        bullet_.transform.position = shotRocation.position;
        bullet_.transform.rotation = shotRocation.rotation;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
