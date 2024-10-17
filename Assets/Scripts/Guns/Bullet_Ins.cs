using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ins : MonoBehaviour
{
    public                                   GameObject bullet;
    private                                  Transform shotRocation;

    private void Awake()
    {
        shotRocation = this.transform;
    }

    public void ShotBulletIns() // 적 유닛 전용. 유저는 레이케이스트 사용
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
