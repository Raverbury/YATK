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
    private int currentLevel = 0;

    private readonly List<GameObject> weaponOrbs = new();

    public override void SetPower(int power, Player player, GameObject weaponOrbPrefab)
    {
        int level = power / 32;
        level = Mathf.Clamp(level, 0, 4);
        if (currentLevel != level)
        {
            ConstructOrbs(level, player, weaponOrbPrefab);
            currentLevel = level;
        }
    }

    private void ConstructOrbs(int level, Player player, GameObject weaponOrbPrefab)
    {
        Player.PlayerPowerUp?.Invoke();
        weaponOrbs.ForEach(orb => GameObject.Destroy(orb));
        weaponOrbs.Clear();
        for (int i = 0; i < level; i++)
        {
            GameObject orb = GameObject.Instantiate(weaponOrbPrefab, parent: player.transform);
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

    public override void Tick(Player player)
    {
        PositionOrbs();
        if (shootFrames > 0)
        {
            if (timeBetweenShot >= shotInterval)
            {
                float baseShotDamage = player.playerData.Attack.GetFinalStat();
                if (isFocused)
                {
                    float focusedOrbDamage = 0.6f * shotDamage;
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, shotDamage, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 4, player.transform.position.y - 4, shotDamage, 20, 90f + 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 4, player.transform.position.y - 4, shotDamage, 20, 90f - 0.3f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x + 16, orb.transform.position.y, focusedOrbDamage, 20, 90f + 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x - 16, orb.transform.position.y, focusedOrbDamage, 20, 90f - 2f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    }
                }
                else
                {
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, shotDamage, 20, 90f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, shotDamage, 20, 90f - 8f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, shotDamage, 20, 90f + 8f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 20, 90f + 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, shotDamage, 20, 90f - 6f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
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
        for (int i = 0; i < weaponOrbs.Count; i++)
        {
            var orb = weaponOrbs[i];
            Vector3 pos = orb.transform.localPosition;
            PositionPair orbPosition = OrbPositions.GetPositionPairAt(currentLevel, i);
            pos = Vector3.MoveTowards(pos, isFocused ? orbPosition.Second : orbPosition.First, 0.08f);
            orb.transform.localPosition = pos;
        }
    }

    public override AbstractShot Clone()
    {
        return new Shot1();
    }

    private static class OrbPositions
    {
        private static readonly PositionPair[][] ORB_POSITIONS = new PositionPair[][] {
            new PositionPair[0],
            new PositionPair[]
            {
                new(new Vector2(0, -0.35f), new Vector2(0, 0.35f)),
            },
            new PositionPair[]
            {
                new(new Vector2(-0.35f, 0), new Vector2(-0.1f, 0.25f)),
                new(new Vector2(0.35f, 0), new Vector2(0.1f, 0.25f)),
            },
            new PositionPair[]
            {
                new(new Vector2(-0.45f, -0.25f), new Vector2(0.2f, 0.25f)),
                new(new Vector2(0, -0.35f), new Vector2(0, 0.35f)),
                new(new Vector2(0.45f, -0.25f), new Vector2(-0.2f, 0.25f)),
            },
            new PositionPair[]
            {
                new(new Vector2(-0.35f, 0), new Vector2(0.1f, 0.25f)),
                new(new Vector2(0.35f, 0), new Vector2(-0.1f, 0.25f)),
                new(new Vector2(0.45f, -0.25f), new Vector2(0.3f, 0)),
                new(new Vector2(-0.45f, -0.25f), new Vector2(-0.3f, 0)),
            },
        };

        /// <summary>
        /// Gets a Vector2 representing local pos of an orb to the player for a certain config.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="orbNumber"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">When given stupid numbers</exception>
        public static PositionPair GetPositionPairAt(int level, int orbNumber)
        {
            return ORB_POSITIONS[level][orbNumber];
        }
    }
}
