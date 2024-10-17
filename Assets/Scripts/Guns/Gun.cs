using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : GunControllerManager
{
    //�ѱ���� �� ���������� ������ �ִ� Ư¡�� �����ϴ� Ŭ����

    //public                              Bullet_Ins bullet_Ins;
    public                              string gunName; // ���� �̸�
    // ���� ���� ���� Inspector â���� �̸� ����.
    public                              bool isShotgun;
    public                              bool isSMG;
    public                              bool isRifle;
    public                              bool isHandGun;
    public                              bool now_Reroading = false; // ���� �� ���� ��, ������ �� ���� �ƴ�.
    public                              float rpm; // ����� �ʴ� ������. ��� �� ���� ��ݱ����� �� �ð� ���� �ִϸ��̼��� �̷����� ��.
    public                              float reRoadTime; // �����ð�
    // Invoke(�Լ�, reRoadTIme); ���� ������ �ð� ����
    public                              int curBullet; // ���� źâ�� ���� �ִ� �Ѿ�
    public                              int remainBullet; // ������ �� �ִ� ���� �ִ� �Ѿ�
    public                              int maxReroadableBullet; // ������ �� źâ�� ���� �Ǵ� �ִ� �Ѿ�

    public override bool Fire(float NowTIme)
    // NowTime���� Time.deltaTime���� �޾ƿ� Ÿ�ӿ�ġ �ð��� ���� ��.
    // user�� Fire.
    {
        if (Input.GetMouseButton(0) && this.rpm <= NowTIme && !this.now_Reroading && this.curBullet <= 0) // Ȯ���� ���� curBullet <= 0���� ����. ���� �۵� �ô� curBullet > 0�� ��.
        {
            --this.curBullet;

            //GameObject effect = Instantiate(muzzleEffect);
            //effect.transform.position = muzzlePoint.position;
            //effect.transform.rotation = muzzlePoint.rotation;

            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out get_hit_info, LayerMask.GetMask("Enemy", "Wall")))
            {
                if (get_hit_info.collider.CompareTag("Enemy"))
                {
                    get_hit_info.collider.GetComponent<FSM>().SetStateDead();
                    Debug.Log("�� ���� Dead");
                }
            }
            return true;
        }
        else return false;
    }

    public override void ReRoad()
    // NowTime���� Time.deltaTime���� �޾ƿ� Ÿ�� ��ġ �ð��� ���� ��.
    // user�� ReRoad. ���� �������� ����.
    {
        if (this.curBullet != this.maxReroadableBullet)
        {
            int ammoNeeded = this.maxReroadableBullet - this.curBullet;
            int ammoToReload = Mathf.Min(ammoNeeded, this.remainBullet);

            this.curBullet += ammoToReload;
            this.remainBullet -= ammoToReload;
        }
    }

    public override bool Enemy_Fire(float NowTIme)
    // NowTime���� Time.deltaTime���� �޾ƿ� Ÿ�ӿ�ġ �ð��� ���� ��.
    // Enemy�� Fire.
    {
        if (this.rpm >= NowTIme)
        {
            bullet_Ins.ShotBulletIns();
            return true;
        }
        else return false;
    }

    private void Awake()
    {
        if (this.gameObject.CompareTag("SMG")) { this.isSMG = true; this.isRifle = false; this.isHandGun = false; this.isShotgun = false; this.gunName = "SMG"; }
        if (this.gameObject.CompareTag("Rifle")) { this.isRifle = true; this.isSMG = false; this.isHandGun = false; this.isShotgun = false; this.gunName = "Rifle"; }
        if (this.gameObject.CompareTag("HandGun")) { this.isHandGun = true; this.isSMG = false; this.isRifle = false; this.isShotgun = false; this.gunName = "HandGun"; }
        if (this.gameObject.CompareTag("Shotgun")) { this.isShotgun = true; this.isSMG = false; this.isRifle = false; this.isHandGun = false; this.gunName = "Shotgun"; }


        if (isSMG)
        {
            this.rpm = float.Parse(SMG[0]);
            this.reRoadTime = float.Parse(SMG[1]);
            this.maxReroadableBullet = int.Parse(SMG[2]);
        }
        if(isHandGun)
        {
            this.rpm = float.Parse(Handgun[0]);
            this.reRoadTime = float.Parse(Handgun[1]);
            this.maxReroadableBullet = int.Parse(Handgun[2]);
        }
        if (isRifle)
        {
            this.rpm = float.Parse(Rifle[0]);
            this.reRoadTime = float.Parse(Rifle[1]);
            this.maxReroadableBullet = int.Parse(Rifle[2]);
        }
        if (isShotgun)
        {
            this.rpm = float.Parse(Shotgun[0]);
            this.reRoadTime = float.Parse(Shotgun[1]);
            this.maxReroadableBullet = int.Parse(Shotgun[2]);
        }
    }

}
