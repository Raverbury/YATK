using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using MEC;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Shot2 : AbstractShot
{
    private readonly List<GameObject> orbs = new();
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private int shotInterval = 6;

    private int timeBetweenShot = 0;
    private int currentLevel = 0;

    readonly List<GameObject> weaponOrbs = new();
    
    public override void SetPower(int power, Player player, GameObject weaponOrbPrefab)
    {
        int level = power / 25;
        level = Mathf.Clamp(level, 0, 5);
        if (currentLevel != level)
        {
            SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_PLAYER_POWERUP, 1f);
            ConstructOrbs(level, player, weaponOrbPrefab);
            currentLevel = level;
        }
    }

    private void ConstructOrbs(int level, Player player, GameObject weaponOrbPrefab)
    {
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

    public override void SetFocus(bool isFocused)
    {
        this.isFocused = isFocused;
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
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 15, player.transform.position.y, baseShotDamage * 0.4f, 20, isFocused ? 90f : 100f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 15, player.transform.position.y, baseShotDamage * 0.4f, 20, isFocused ? 90f : 80f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 10, player.transform.position.y, baseShotDamage * 0.4f, 20, 90f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 10, player.transform.position.y, baseShotDamage * 0.4f, 20, 90f, STG.PlayerShotType.IN_REIMU_AMULET_RED, 0);
                if (isFocused)
                {
                    for (int i = 0; i < weaponOrbs.Count; i++)
                    {
                        var orb = weaponOrbs[i];
                        Entity entity = ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.3f, 8, 90f, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                        CoroutineUtil.RunEntityBoundCoroutine(_DoHoming(entity, player), entity);
                    }
                }
                else
                {
                    float spread = 10f;
                    float halfFanSpread = spread * (weaponOrbs.Count - 1) * 0.5f;
                    for (int i = 0; i < weaponOrbs.Count; i++)
                    {
                        var orb = weaponOrbs[i];
                        Entity entity = ECSEntitySpawner.SpawnPlayerBulletP1(orb.transform.position.x, orb.transform.position.y, baseShotDamage * 0.3f, 8, 90f + halfFanSpread - spread * i, STG.PlayerShotType.IN_REIMU_AMULET_BLUE, 0);
                        CoroutineUtil.RunEntityBoundCoroutine(_DoHoming(entity, player), entity);
                    }
                }
                timeBetweenShot = 0;
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_PLAYER_SHOOT, 1f);
            }
            shootFrames -= 1;
        }
        timeBetweenShot = Mathf.Min(timeBetweenShot + 1, shotInterval);
    }

    private void PositionOrbs()
    {
        float distance = 0.15f;
        float xOffset = -(weaponOrbs.Count - 1) * distance / 2f;
        for (int i = 0; i < weaponOrbs.Count; i++)
        {
            var orb = weaponOrbs[i];
            float xPos = xOffset + distance * i;
            Vector3 pos = orb.transform.localPosition;
            pos = Vector3.MoveTowards(pos, !isFocused ? new Vector2(xPos * 1.2f, -0.35f) : new Vector2(xPos, 0.35f), 0.1f);
            orb.transform.localPosition = pos;
        }
    }

    IEnumerator<float> _DoHoming(Entity entity, Player player)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        int t = 0;
        bool alreadyHome = false;
        GameObject homingTarget = null;
        while (!ECSEntitySpawner.EntityIsDisabled(entity))
        {
            GameObject[] enemies = StageManager.instance.GetTargetableEnemies();
            float shortestDistance = 10000f;
            if (homingTarget != null)
            {
                alreadyHome = true;
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
                float angle = math.degrees(math.Euler(localTransform.Rotation).z);
                float homingAngle = Mathf.Rad2Deg * Mathf.Atan2(
                    homingTarget.transform.position.y - localTransform.Position.y,
                    homingTarget.transform.position.x - localTransform.Position.x
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
                localTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(angle)));
                entityManager.SetComponentData(entity, localTransform);
            }
            else
            {
                if (!alreadyHome)
                {
                    if (enemies.Count() == 1)
                    {
                        homingTarget = enemies[0];
                    }
                    else
                    {
                        for (int i = 0; i < enemies.Count(); i++)
                        {
                            float distance = Vector2.Distance(enemies[i].transform.position, entity.Position());
                            if (distance < shortestDistance)
                            {
                                shortestDistance = distance;
                                homingTarget = enemies[i];
                            }
                        }
                    }
                }
            }
            yield return Timing.WaitForOneFrame;
            t += 1;
        }

    }

    public override AbstractShot Clone()
    {
        return new Shot2();
    }

    public override string ShotName()
    {
        return "Bewitching Amulet";
    }

    public override string ShotDescription()
    {
        return "Homing shot";
    }

    public override Color ShotColor()
    {
        return new Color(255 / 255, 51f / 255f, 51f / 255);
    }
}
