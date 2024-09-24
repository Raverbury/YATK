using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class PlayerBulletPool : ObjectPool
{
    private static PlayerBulletPool instance = null;

    protected override void AltAwake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    public static GameObject SpawnBulletP1(float x, float y, float damage, float speed, float angle, PlayerShotType playerShotType, int delay)
    {
        GameObject bullet = instance.RequestObject();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.speed = speed;
        playerBullet.damage = damage;
        ShotData playerShotData = ShotSheet.GetPlayerShotData((int)playerShotType);
        playerBullet.SetGraphic(playerShotData.SPRITES[0], playerShotData.size, playerShotData.hitboxRadius);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, delay));
        return bullet;
    }

    private static IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        bullet.SetActive(true);
    }
}