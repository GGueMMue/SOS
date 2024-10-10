using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GunControllerManager : MonoBehaviour
{
    //[SerializeField]                    public Gun now_gun;
    [SerializeField] protected           GameObject muzzleEffect;
    [SerializeField] protected           Transform muzzlePoint;
    [SerializeField] protected           RaycastHit get_hit_info;
    [SerializeField] protected           Bullet_Ins bullet_Ins;

    [SerializeField] protected string[] SMG = new string[] {"0.125", "1.8", "30"};
    [SerializeField] protected string[] Handgun = new string[] { "0.35", "1.1", "12" };
    [SerializeField] protected string[] Rifle = new string[] { "0.25", "2.3", "25" };
    [SerializeField] protected string[] Shotgun = new string[] { "1.8", "3.5", "6" };

    abstract public bool Fire(float NowTIme);
    abstract public void ReRoad();
    abstract public bool Enemy_Fire(float NowTIme);

    /*protected void Fire(float NowTIme) 
        // NowTime에는 Time.deltaTime으로 받아온 타임워치 시간이 들어가야 함.
        // user의 Fire.
    {
        if(Input.GetMouseButton(0) && now_gun.rpm >= NowTIme && !now_gun.now_Reroading && now_gun.remainBullet <= 0)
        {
            --now_gun.remainBullet;

            GameObject effect = Instantiate(muzzleEffect);
            effect.transform.position = muzzlePoint.position;
            effect.transform.rotation = muzzlePoint.rotation;

            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out get_hit_info, LayerMask.GetMask("Enemy", "Wall")))
            {
                if (get_hit_info.collider.CompareTag("Enemy"))
                {
                    Debug.Log("적 상태 Dead");
                }
            }
        }
    }

    protected void ReRoad()
        // NowTime에는 Time.deltaTime으로 받아온 타임 워치 시간이 들어가야 함.
        // user의 ReRoad. 적은 장전하지 않음.
    {
        if (Input.GetKeyDown(KeyCode.R) && now_gun.curBullet != now_gun.maxReroadableBullet)
        {
            int ammoNeeded = now_gun.maxReroadableBullet - now_gun.curBullet;
            int ammoToReload = Mathf.Min(ammoNeeded, now_gun.remainBullet);

            now_gun.curBullet += ammoToReload;
            now_gun.remainBullet -= ammoToReload;
        }
    }

    protected void Enemy_Fire(float NowTIme)
    // NowTime에는 Time.deltaTime으로 받아온 타임워치 시간이 들어가야 함.
    // Enemy의 Fire.
    {
        if(now_gun.rpm >= NowTIme)
            bullet_Ins.ShotBulletIns();
    }*/
}
