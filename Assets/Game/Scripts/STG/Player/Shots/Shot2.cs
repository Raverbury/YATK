using System;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using UnityEngine;

public class Shot2 : AbstractShot
{
    private readonly List<GameObject> orbs = new();
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private int shotInterval = 6;
    private float shotDamage = 6;

    private int timeBetweenShot = 0;
    private bool isFocused = false;
    private int currentLevel = 0;

    List<GameObject> weaponOrbs = new();

    protected override void CalcShotInterval()
    {
        shotInterval = Mathf.CeilToInt(300f / (5f + player.playerData.RateOfFire.GetFinalStat()));
    }

    protected override void CalcShotDamage()
    {
        shotDamage = 0.6f * player.playerData.Attack.GetFinalStat();
    }

    public override void SetPower(int power)
    {
        int level = power / 25;
        level = Mathf.Clamp(level, 0, 5);
        if (currentLevel != level)
        {
            ConstructOrbs(level);
            currentLevel = level;
        }
    }

    private void ConstructOrbs(int level)
    {
        Player.PlayerPowerUp?.Invoke();
        weaponOrbs.ForEach(orb => Destroy(orb));
        weaponOrbs.Clear();
        for (int i = 0; i < level; i++)
        {
            GameObject orb = Instantiate(player.weaponOrb, transform);
            if (orb.TryGetComponent(out AutoRotate autoRotate))
            {
                autoRotate.rotateSpeed *= (0 == i % 2) ? 1 : -1;
            }
            weaponOrbs.Add(orb);
        }
    }

    public override void Shoot()
    {
        shootFrames = SHOOT_FRAMES;
    }

    public override void SetFocus(bool isFocused)
    {
        this.isFocused = isFocused;
    }

    protected override void PausableUpdate()
    {
        PositionOrbs();
        if (shootFrames > 0)
        {
            if (timeBetweenShot >= shotInterval)
            {
                CalcShotInterval();
                CalcShotDamage();
                float baseAmuletDamage = 1.6f * shotDamage;
                PlayerBulletPool.SpawnBulletP1(transform.position.x - 10, transform.position.y, baseAmuletDamage, 20, isFocused ? 90f : 91f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                PlayerBulletPool.SpawnBulletP1(transform.position.x + 10, transform.position.y, baseAmuletDamage, 20, isFocused ? 90f : 89f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                if (isFocused)
                {
                    foreach (var orb in weaponOrbs)
                    {
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 25, 90f - 2f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                    }
                }
                else
                {
                    // Timing.RunCoroutine(_Manipulate(b1));
                    // Timing.RunCoroutine(_Manipulate(b2));
                    foreach (var orb in weaponOrbs)
                    {
                        GameObject bulletGameObject = PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 8, 90f - 6f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                        Timing.RunCoroutine(_DoHoming(bulletGameObject));
                    }
                }
                timeBetweenShot = 0;
            }
            shootFrames -= 1;
        }
        timeBetweenShot = Mathf.Min(timeBetweenShot + 1, shotInterval);
    }

    private void PositionOrbs()
    {
        int i = 0;
        float distance = 0.15f;
        float xOffset = -(weaponOrbs.Count - 1) * distance / 2f;
        foreach (var orb in weaponOrbs)
        {
            float xPos = xOffset + distance * i;
            Vector3 pos = orb.transform.localPosition;
            pos = Vector3.MoveTowards(pos, !isFocused ? new Vector2(xPos * 1.2f, -0.35f) : new Vector2(xPos, 0.35f), 0.06f);
            orb.transform.localPosition = pos;
            i++;
        }
    }

    IEnumerator<float> _DoHoming(GameObject bulletGameObject)
    {
        if (bulletGameObject.TryGetComponent(out PlayerBullet playerBullet))
        {
            int t = 0;
            while (bulletGameObject.activeInHierarchy)
            {
                GameObject homingTarget = StageManager.instance.GetFirstEnemy();
                if (homingTarget != null)
                {
                    float angle = bulletGameObject.transform.eulerAngles.z;
                    float homingAngle = Mathf.Rad2Deg * Mathf.Atan2(
                        homingTarget.transform.position.y - bulletGameObject.transform.position.y,
                        homingTarget.transform.position.x - bulletGameObject.transform.position.x
                    );
                    // if (homingAngle < 0) {
                    //     homingAngle = 360f + homingAngle;
                    // }
                    // Debug.Log(homingAngle);
                    float diff = homingAngle - angle;
                    while (diff >= 180f)
                    {
                        diff -= 360f;
                    }
                    while (diff < -180f)
                    {
                        diff += 360f;
                    }
                    float diffAbs = Mathf.Abs(diff);
                    if (diffAbs <= 2f)
                    {
                        angle = homingAngle;
                    }
                    else if (diffAbs > 2f)
                    {
                        angle += diffAbs / 10f * diff / (diffAbs / 2f);
                    }
                    bulletGameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);
                }
                yield return Timing.WaitForOneFrame;
                t += 1;
            }
        }
    }
}
