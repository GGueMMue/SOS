using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Ins : MonoBehaviour
{
    public                                   GameObject bullet;
    private                                  Transform shotRocation;

    private void Awake()
    {
        shotRocation = this.transform;

    }

    public void ShotBulletIns() // �� ���� ����. ������ �������̽�Ʈ ���
    {
        GameObject bullet_ = Instantiate(bullet);
        bullet_.transform.position = shotRocation.position;
        bullet_.transform.rotation = shotRocation.rotation;
    }

    public void ShotgunBulletIns(float rot) //, Vector3 spwanOffset) ���� ��� ź�� �Լ�, �� ����. ������ �����ɽ�Ʈ ���
    {
        GameObject bullet_ = Instantiate(bullet, shotRocation.position, shotRocation.rotation);

        bullet_.transform.Rotate(Vector3.up, rot);

        //Vector3 bulletDirection = bullet_.transform.forward;
        //bullet_.GetComponent<Rigidbody>().velocity = bulletDirection * bullet.GetComponent<Bullet>().force;
        //bullet.transform.rotation = shotRocation.rotation * Quaternion.Euler(0, rot, 0); <- ���� �߸���
        //bullet.transform.rotation = shotRocation.rotation * Quaternion.AngleAxis(rot, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
