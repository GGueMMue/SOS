using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Gun : GunControllerManager
{  
    public float[] shotgunRotationPos = { -10, 5, 0, 5, 10 };

    public AudioClip gun_Sound;
    public AudioClip sg_Sound;
    AudioSource SFX;

    //총기들이 다 공통적으로 가지고 있는 특징을 포함하는 클래스
    /*private Vector3[] spwanOffsets = new Vector3[]
    {
        new Vector3 (-.1f , 0, 0),
        new Vector3 (-.05f, 0, 0),
        new Vector3 (.1f, 0, 0),
        new Vector3 (.05f, 0, 0)
    };*/

    private void Update()
    {
        //Debug.Log(spwanOffsets.Length);
        Debug.Log (shotgunRotationPos.Length);
    }

    private                             FSM fsm;

    //public GameObject muzzle_Effect;
    public Transform muzzleEffectTR;
    //public                              Bullet_Ins bullet_Ins;
    public                              string gunName; // 무기 이름
    public                              bool coroutineChecker = false;
    // 현재 무기 유형 Inspector 창에서 미리 설정.
    public                              bool isShotgun;
    public                              bool isSMG;
    public                              bool isRifle;
    public                              bool isHandGun;
    public                              bool now_Reroading = false; // 참일 때 장전 중, 거짓일 때 장전 아님.
    public                              float rpm; // 연사력 초당 데미지. 사격 후 다음 사격까지의 그 시간 내에 애니메이션이 이뤄져야 함.
    public                              float reRoadTime; // 장전시간
    // Invoke(함수, reRoadTIme); 으로 재장전 시간 구현
    public                              int curBullet; // 현재 탄창에 남아 있는 총알
    public                              int remainBullet; // 장전할 수 있는 남아 있는 총알
    public                              int maxReroadableBullet; // 재장전 시 탄창에 삽입 되는 최대 총알

    //NavMeshAgent nav;

    public bool Fire(float NowTIme, Transform muzzlePoint)
    // NowTime에는 Time.deltaTime으로 받아온 타임워치 시간이 들어가야 함.
    // user의 Fire.
    {
        if (Input.GetMouseButton(0) && this.rpm <= NowTIme && !this.now_Reroading && this.curBullet <= 0) // 확인을 위해 curBullet <= 0으로 지정. 실제 작동 시는 curBullet > 0이 됨.
        {
            --this.curBullet;
            //GunSoundEffect(this.gunName);
            GunSFX();


            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out get_hit_info, LayerMask.GetMask("Enemy", "Wall")))
            {
                if (get_hit_info.collider.CompareTag("Enemy"))
                {
                    get_hit_info.collider.GetComponent<FSM>().SetStateDead();
                    Debug.Log("적 상태 Dead");
                }
            }
            return true;
        }
        else return false;
    }

    public bool ShotGunFire(float nowTime, Transform muzzlePoint)
    {
        if (Input.GetMouseButton(0) && this.rpm <= nowTime && !this.now_Reroading && this.curBullet <= 0) // 확인을 위해 curBullet <= 0으로 지정. 실제 작동 시는 curBullet > 0이 됨.
        {
            --this.curBullet;
            //GunSoundEffect(this.gunName);
            GunShotGunSFX();

            RaycastHit[] hits;
            hits = Physics.SphereCastAll(muzzlePoint.position, 4f, muzzlePoint.forward, 10f, LayerMask.GetMask("Enemy", "Wall"));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<FSM>().SetStateDead();
                }
            }

            return true;
        }
        else return false;
    }

    public override void ReRoad()
    // NowTime에는 Time.deltaTime으로 받아온 타임 워치 시간이 들어가야 함.
    // user의 ReRoad. 적은 장전하지 않음.
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
    // NowTime에는 Time.deltaTime으로 받아온 타임워치 시간이 들어가야 함.
    // Enemy의 Fire.
    {

        if (this.rpm <= NowTIme)
        {
            Debug.Log("쏨");

            bullet_Ins.ShotBulletIns();
            return true;
        }
        else return false;
    } // 사용안함. IEnumerator의 적 공격을 사용

    void MuzzleEffectFunction()
    {
        GameObject effect = Instantiate(muzzleEffect);
        effect.transform.position = muzzleEffectTR.position;
        effect.transform.rotation = muzzleEffectTR.rotation;
        Destroy(effect, 0.2f);
    }


    public void Playe_MuzzleEffectFunction(Transform playerMuzzleTR, GameObject playerMuzzleEffect)
    {
        GameObject effect = Instantiate(playerMuzzleEffect);
        effect.transform.position = playerMuzzleTR.position;
        effect.transform.rotation = playerMuzzleTR.rotation;
        Destroy(effect, 0.2f);
    }

    public IEnumerator Enemy_fire() // 총기 사격 함수, 적 전용
    {
        //if (coroutineChecker) yield break;
        //else
        //{
        //    Debug.Log("호출 확인");

        //    bullet_Ins.ShotBulletIns();

        //    yield return new WaitForSeconds(this.rpm * 10);

        //    coroutineChecker = true;
        //}


        while (fsm.state == FSM.STATE.ATTACK)
        {
            if (!coroutineChecker)
            {
                Debug.Log("호출 확인");
                MuzzleEffectFunction();
               
                bullet_Ins.ShotBulletIns();
                GunSFX();

                coroutineChecker = true;
                //nav.isStopped = true;

                yield return new WaitForSeconds(this.rpm);
                coroutineChecker = false;
                //nav.isStopped = false;

            }
            else
            {
                yield return null;
            }
        }
        //StartCoroutine(Enemy_fire());
    }

    public IEnumerator Enemy_Shotgun_Fire() // 샷건 사격 함수 적 전용
    {
        while (fsm.state == FSM.STATE.ATTACK)
        {
            if (!coroutineChecker)
            {
                for (int i = 0; i < shotgunRotationPos.Length; i++)
                {
                    Debug.Log("샷건 호출 확인");
                    MuzzleEffectFunction();
                    GunShotGunSFX();

                    bullet_Ins.ShotgunBulletIns(shotgunRotationPos[i]); //, spwanOffsets[i]);

                    //yield return new WaitForFixedUpdate();
                }
                coroutineChecker = true;
                //nav.isStopped = true;
                yield return new WaitForSeconds(this.rpm);
                //nav.isStopped = false;
                coroutineChecker = false;
            }
            else { yield return null; }

        }
    }
    private void Start()
    {
        fsm = GetComponentInParent<FSM>();

        //nav = GetComponentInParent<NavMeshAgent>();

        SFX = GetComponent<AudioSource>();
        SFX.volume = .5f;
        GunSetUp();
    }

    public void SetPickUpGunSetting()
    {
        GunSetUp();

        fsm = GetComponentInParent<FSM>();

        //nav = GetComponentInParent<NavMeshAgent>();

        SFX = GetComponent<AudioSource>();
        SFX.volume = .5f;

        this.curBullet = Random.Range(5, this.maxReroadableBullet);

    }

    public void GunShotGunSFX()
    {
        SFX.PlayOneShot(gun_Sound);
    }

    public void GunSFX()
    {
        SFX.PlayOneShot(sg_Sound);
    }

    public void GunSetUp()
    {
        if (this.gameObject.CompareTag("SMG")) { this.isSMG = true; this.isRifle = false; this.isHandGun = false; this.isShotgun = false; this.gunName = "SMG"; }
        if (this.gameObject.CompareTag("Rifle")) { this.isRifle = true; this.isSMG = false; this.isHandGun = false; this.isShotgun = false; this.gunName = "Rifle"; }
        if (this.gameObject.CompareTag("HandGun")) { this.isHandGun = true; this.isSMG = false; this.isRifle = false; this.isShotgun = false; this.gunName = "HandGun"; }
        if (this.gameObject.CompareTag("Shotgun")) { this.isShotgun = true; this.isSMG = false; this.isRifle = false; this.isHandGun = false; this.gunName = "Shotgun"; }
        if (this.gameObject.CompareTag("Untagged")) this.gunName = "Meele";

        if (isSMG)
        {
            this.rpm = float.Parse(SMG[0]);
            this.reRoadTime = float.Parse(SMG[1]);
            this.maxReroadableBullet = int.Parse(SMG[2]);
        }
        if (isHandGun)
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
