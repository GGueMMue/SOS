using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : GunControllerManager
{
    public SMG()
    {
        this.now_gun.isSMG = true;
        this.now_gun.isHandGun = false;
        this.now_gun.isRifle = false;
        this.now_gun.isShotgun = false;
        this.now_gun.maxReroadableBullet = 30;
        this.now_gun.reRoadTime = 1.7f;
        this.now_gun.rpm = 0.12f;
    }
}
