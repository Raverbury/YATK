using System;
using System.Collections.Generic;
using UnityEngine;

public class Shot1 : AbstractShot
{
    private readonly List<GameObject> orbs = new();
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private int shotInterval = 6;
    private float shotDamage = 6;

    private int timeBetweenShot = 0;
    private bool isFocused = false;
    private int currentLevel = 0;

    private readonly Dictionary<int, List<Tuple<Vector2, Vector2>>> ORB_POSITIONS = new()
        {
            {0, new()},
            {1, new()
                {
                    new(new Vector2(0, -0.35f), new Vector2(0, 0.35f)),
                }
            },
            {2, new()
                {
                    new(new Vector2(-0.35f, 0), new Vector2(-0.1f, 0.25f)),
                    new(new Vector2(0.35f, 0), new Vector2(0.1f, 0.25f)),
                }
            },
            {3, new()
                {
                    new(new Vector2(-0.45f, -0.25f), new Vector2(0.2f, 0.25f)),
                    new(new Vector2(0, -0.35f), new Vector2(0, 0.35f)),
                    new(new Vector2(0.45f, -0.25f), new Vector2(-0.2f, 0.25f)),
                }
            },
            {4, new()
                {
                    new(new Vector2(-0.35f, 0), new Vector2(0.1f, 0.25f)),
                    new(new Vector2(0.35f, 0), new Vector2(-0.1f, 0.25f)),
                    new(new Vector2(0.45f, -0.25f), new Vector2(0.3f, 0)),
                    new(new Vector2(-0.45f, -0.25f), new Vector2(-0.3f, 0)),
                }
            },
        };

    List<GameObject> weaponOrbs = new();

    protected override void CalcShotInterval()
    {
        shotInterval = Mathf.CeilToInt(300f / (5f + player.playerData.RateOfFire.GetFinalStat()));
    }

    protected override void CalcShotDamage()
    {
        shotDamage = 0.8f * player.playerData.Attack.GetFinalStat();
    }

    public override void SetPower(int power)
    {
        int level = power / 32;
        level = Mathf.Clamp(level, 0, 4);
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
                if (isFocused)
                {
                    float focusedOrbDamage = 0.6f * shotDamage;
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, shotDamage, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x + 4, transform.position.y - 4, shotDamage, 20, 90f + 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x - 4, transform.position.y - 4, shotDamage, 20, 90f - 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x + 16, orb.transform.position.y, focusedOrbDamage, 20, 90f + 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x - 16, orb.transform.position.y, focusedOrbDamage, 20, 90f - 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    }
                }
                else
                {
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, shotDamage, 20, 90f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, shotDamage, 20, 90f - 8f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, shotDamage, 20, 90f + 8f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 20, 90f + 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 20, 90f - 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
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
        foreach (var orb in weaponOrbs)
        {
            Vector3 pos = orb.transform.localPosition;
            Tuple<Vector2, Vector2> orbPosition = ORB_POSITIONS[currentLevel][i];
            pos = Vector3.MoveTowards(pos, isFocused ? orbPosition.Item2 : orbPosition.Item1, 0.08f);
            orb.transform.localPosition = pos;
            i++;
        }
    }
}
