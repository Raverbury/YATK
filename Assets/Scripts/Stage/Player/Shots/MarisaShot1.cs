using System.Collections.Generic;
using MEC;
using Unity.Entities;
using UnityEngine;

public class MarisaShot1 : AbstractShot
{
    private int shootFrames = 0;
    private const int SHOOT_FRAMES = 30;
    private int shotInterval = 6;
    private int missileInterval = 6;

    private int shotCooldown = 0;
    private int missileCooldown = 0;
    private int currentLevel = 0;

    public override void SetPower(int power, Player player, GameObject weaponOrbPrefab)
    {
        int level = power / 32;
        level = Mathf.Clamp(level, 0, 4);
        if (level != currentLevel)
        {
            Player.PlayerPowerUp?.Invoke();
            currentLevel = level;
        }
    }

    public override void Shoot()
    {
        shootFrames = SHOOT_FRAMES;
    }

    public override void Tick(Player player)
    {
        if (shootFrames > 0)
        {
            float baseShotDamage = player.playerData.Attack.GetFinalStat();
            shotInterval = ROFScaling.GetFramesBetweenShot((int)player.playerData.RateOfFire.GetFinalStat());
            missileInterval = shotInterval * (5 - currentLevel);
            if (shotCooldown <= 0)
            {
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x + 4f, player.transform.position.y, baseShotDamage, 20, 90f, STG.PlayerShotType.IN_MARISA_MISSILE_BLUE, 0);
                ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x - 4f, player.transform.position.y, baseShotDamage, 20, 90f, STG.PlayerShotType.IN_MARISA_MISSILE_BLUE, 0);
                if (!isFocused)
                {
                    const float SPREAD = 8f;
                    float halfFanSpread = SPREAD * (currentLevel - 1) / 2f;
                    for (int i = 0; i < currentLevel; i++)
                    {
                        ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y - 2f, baseShotDamage * 0.2f, 20f, 90f - halfFanSpread + SPREAD * i, STG.PlayerShotType.IN_MARISA_MISSILE_RED, 0);
                    }
                }
                shotCooldown = shotInterval;
            }
            if (missileCooldown <= 0)
            {
                if (isFocused && currentLevel > 0)
                {
                    Entity entity = ECSEntitySpawner.SpawnPlayerBulletP1(player.transform.position.x, player.transform.position.y - 2f, baseShotDamage * 2.5f, -0.1f, 90f, STG.PlayerShotType.IN_MARISA_FOCUSED_MISSILE, 0);
                    CoroutineUtil.RunEntityBoundCoroutine(_DoDelay(entity), entity);
                    missileCooldown = missileInterval;
                }
            }
            shootFrames -= 1;
        }
        shotCooldown = shotCooldown < 1 ? 0 : shotCooldown - 1;
        missileCooldown = missileCooldown < 1 ? 0 : missileCooldown - 1;
    }

    private IEnumerator<float> _DoDelay(Entity missile)
    {
        yield return WaitForFrames.WaitWrapper(20);
        for (int i = 0; i < 15; i++)
        {
            ECSEntitySpawner.SetBulletSpeed(missile, i);
            yield return Timing.WaitForOneFrame;
        }
    }

    public override AbstractShot Clone()
    {
        return new MarisaShot1();
    }

    public override string ShotName()
    {
        return "Magic Missile";
    }

    public override string ShotDescription()
    {
        return "Tight-spread powerful and delayed shot";
    }

    public override Color ShotColor()
    {
        return new Color(255f / 255, 204f / 255f, 0f);
    }
}
