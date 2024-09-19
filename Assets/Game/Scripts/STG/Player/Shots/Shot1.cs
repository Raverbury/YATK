using System;
using System.Collections.Generic;
using UnityEngine;

public class Shot1 : AbstractShot
{
    private readonly List<GameObject> orbs = new();
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private const int SHOOT_INTERVAL = 6;
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
                    new(new Vector2(0.65f, -0.25f), new Vector2(0.3f, 0)),
                    new(new Vector2(-0.65f, -0.25f), new Vector2(-0.3f, 0)),
                }
            },
        };

    List<GameObject> weaponOrbs = new();

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

    private void Update()
    {
        PositionOrbs();
        if (shootFrames > 0)
        {
            // TODO: shoot
            if (timeBetweenShot >= SHOOT_INTERVAL)
            {
                if (isFocused)
                {
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x + 2, transform.position.y, 20, 90f + 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x - 2, transform.position.y, 20, 90f - 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x + 16, orb.transform.position.y, 20, 90f + 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x - 16, orb.transform.position.y, 20, 90f - 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    }
                }
                else
                {
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, 20, 90f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, 20, 90f - 15f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    PlayerBulletPool.SpawnBulletP1(transform.position.x, transform.position.y, 20, 90f + 15f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, 20, 90f + 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                        PlayerBulletPool.SpawnBulletP1(orb.transform.position.x, orb.transform.position.y, 20, 90f - 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                    }
                }
                timeBetweenShot = 0;
            }
            shootFrames -= 1;
        }
        timeBetweenShot = Mathf.Min(timeBetweenShot + 1, SHOOT_INTERVAL);
    }

    private void PositionOrbs()
    {
        int i = 0;
        foreach (var orb in weaponOrbs)
        {
            Vector3 pos = orb.transform.localPosition;
            Tuple<Vector2, Vector2> orbPosition = ORB_POSITIONS[currentLevel][i];
            pos = Vector3.MoveTowards(pos, isFocused ? orbPosition.Item2 : orbPosition.Item1, 0.06f);
            orb.transform.localPosition = pos;
            i++;
        }
    }
}
