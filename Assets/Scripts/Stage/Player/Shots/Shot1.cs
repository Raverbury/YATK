using System;
using System.Collections.Generic;
using UnityEngine;

public class Shot1 : AbstractShot
{
    private readonly List<GameObject> orbs = new();
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private int shotInterval = 6;

    private int timeBetweenShot = 0;
    private int currentLevel = 0;

    private readonly List<GameObject> weaponOrbs = new();
    private float orbRotation = 0f;

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
                shotInterval = ROFScaling.GetFramesBetweenShot((int)player.playerData.RateOfFire.GetFinalStat());
                if (isFocused)
                {
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, baseShotDamage * 0.7f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 4f, player.transform.position.y - 4, baseShotDamage * 0.7f, 20, 90f - 4f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 4f, player.transform.position.y - 4, baseShotDamage * 0.7f, 20, 90f + 4f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x + 6f, orb.transform.position.y, baseShotDamage * 0.25f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x - 6f, orb.transform.position.y, baseShotDamage * 0.25f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x + 6f, orb.transform.position.y, baseShotDamage * 0.25f, 20, -90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x - 6f, orb.transform.position.y, baseShotDamage * 0.25f, 20, -90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                    }
                }
                else
                {
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y, baseShotDamage * 0.7f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 10f, player.transform.position.y, baseShotDamage * 0.7f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 10f, player.transform.position.y, baseShotDamage * 0.7f, 20, 90f, STG.PlayerShotType.IN_YUKARI_NEEDLE_PURPLE, 0);
                    foreach (var orb in weaponOrbs)
                    {
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.25f, 20, 90f + 12f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.25f, 20, 90f - 12f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.25f, 20, -90f + 12f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
                        ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.25f, 20, -90f - 12f, STG.PlayerShotType.IN_YUKARI_NEEDLE_YELLOW, 0);
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
        const float FOCUSED_RADIUS = 0.3f;
        const float UNFOCUSED_RADIUS = 0.45f;
        orbRotation -= 2.13f;
        orbRotation %= 360f;
        float orbRotationRad = orbRotation * Mathf.Deg2Rad;
        float spread = 2 * Mathf.PI / weaponOrbs.Count;
        for (int i = 0; i < weaponOrbs.Count; i++)
        {
            var orb = weaponOrbs[i];
            Vector3 pos = orb.transform.localPosition;
            Vector3 focusedPos = new Vector3(FOCUSED_RADIUS * Mathf.Cos(orbRotationRad + spread * i), FOCUSED_RADIUS * Mathf.Sin(orbRotationRad + spread * i));
            Vector3 unfocusedPos = new Vector3(UNFOCUSED_RADIUS * Mathf.Cos(orbRotationRad + spread * i), UNFOCUSED_RADIUS * Mathf.Sin(orbRotationRad + spread * i));
            pos = Vector3.MoveTowards(pos, isFocused ? focusedPos : unfocusedPos, 0.02f);
            orb.transform.localPosition = pos;
        }
    }

    public override AbstractShot Clone()
    {
        return new Shot1();
    }

    public override string ShotName()
    {
        return "Phantasmic Boundary";
    }

    public override string ShotDescription()
    {
        return "Tricky rotating needles";
    }

    public override Color ShotColor()
    {
        return new Color(204f / 255, 102f / 255f, 1f);
    }
}
