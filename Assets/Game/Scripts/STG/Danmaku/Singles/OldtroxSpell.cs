using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class OldtroxSpell : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
    }

    public override string GetName()
    {
        return "Dark Kin [Dark Flight of Torments]";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 60;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            enemy.SetAnimState(Enemy.AnimState.Attack);
            yield return WaitForFrames.WaitWrapper(10);
            FireBladesOfTorment(enemy.transform.position);
            yield return WaitForFrames.WaitWrapper(70);
            FireBladesOfTorment(enemy.transform.position);
            yield return WaitForFrames.WaitWrapper(30);
            FireBladesOfTorment(enemy.transform.position);
            // FireRing(enemy.transform.position, 30);
            yield return WaitForFrames.WaitWrapper(60);
            Vector2 playerPos = new Vector2(192f, -360f);
            if (Player.instance != null)
            {
                playerPos = Player.instance.transform.position;
            }
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_CastDarkFlight(playerPos, enemy)));
            yield return WaitForFrames.WaitWrapper(100);
        }
    }

    private void FireRing(Vector2 pos, int branch)
    {
        float r = UnityEngine.Random.Range(0f, 360f);
        float gap = 360f / branch;
        for (int i = 0; i < branch; i++)
        {
            ECSEntitySpawner.SpawnEnemyBulletE1(pos, 6f, r + gap * i, EnemyBulletType.AMULET_DARK_BLUE, 10);
        }
    }

    private void FireBladesOfTorment(Vector2 pos)
    {
        float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
            Player.instance.transform.position.y - pos.y,
            Player.instance.transform.position.x - pos.x
        );
        const int BURSTS = 11;
        const int BRANCHES = 1;
        const float SPREAD = 9f;
        const float HALF_FAN_SPREAD = SPREAD * (BRANCHES - 1) * 0.5f;
        const float RADIUS = 50f;
        for (int i = 0; i < 2; i++)
        {
            float sideAngle = Mathf.Deg2Rad * (angleToPlayer - 90f + 180f * i);
            float bulletSpawnX = pos.x + Mathf.Cos(sideAngle) * RADIUS;
            float bulletSpawnY = pos.y + Mathf.Sin(sideAngle) * RADIUS;
            float aimAngle = Mathf.Rad2Deg * Mathf.Atan2(
                Player.instance.transform.position.y - bulletSpawnY,
                Player.instance.transform.position.x - bulletSpawnX
            );
            for (int j = 0; j < BURSTS; j++)
            {
                for (int k = 0; k < BRANCHES; k++)
                {
                    float trueAimAngle = aimAngle - HALF_FAN_SPREAD + SPREAD * k;
                    Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletSpawnX, bulletSpawnY, 4f, trueAimAngle, j switch
                    {
                        0 or 1 or 2 => EnemyBulletType.ARROW_DARK_RED,
                        _ => EnemyBulletType.ARROW_BLACK,
                    }, 10 + 3 * j);
                    if (j == 0)
                    {
                        CoroutineUtil.RunEntityBoundCoroutine(_SpawnBladesOfTormentTrail(entity, trueAimAngle), entity);
                    }
                }
            }
        }
    }

    private IEnumerator<float> _SpawnBladesOfTormentTrail(Entity entity, float facing)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        while (true)
        {
            float3 localPos = entityManager.GetComponentData<LocalTransform>(entity).Position;
            Vector2 spawnPos = new Vector2(localPos.x, localPos.y);
            Entity trailEntity = ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, 0f, facing, EnemyBulletType.AMULET_RED, 10);
            CoroutineUtil.RunEntityBoundCoroutine(_DespawnStationaryEntity(trailEntity, 90), trailEntity);
            yield return WaitForFrames.WaitWrapper(5);
        }
    }

    private IEnumerator<float> _CastDarkFlight(Vector2 targetPos, Enemy enemy)
    {
        const int OUTER_BRANCHES = 40;
        const int BRANCHES = 100;
        const int FLIGHT_PREP_TIME = 40;
        const int FLIGHT_TIME = 10;
        const int SPAWN_DELAY = FLIGHT_PREP_TIME + FLIGHT_TIME;

        // spawn warning outer rings warning indicator
        float gap = 360f / OUTER_BRANCHES;
        for (int i = 0; i < 2; i++)
        {
            float radius = 20f + 35f * i;
            for (int j = 0; j < OUTER_BRANCHES; j++)
            {
                float angle = gap * j;
                float spawnPosX = targetPos.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float spawnPosY = targetPos.y + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(spawnPosX, spawnPosY, 0f, angle - 180f, EnemyBulletType.BALL2_BLUE, SPAWN_DELAY);
                CoroutineUtil.RunEntityBoundCoroutine(_SpreadDarkFlightOuterRing(entity, SPAWN_DELAY + 120 * (i % 2)), entity);
            }
        }

        // back away
        Vector2 behindVector = new Vector2(enemy.transform.position.x, enemy.transform.position.y) - targetPos;
        behindVector.Normalize();
        Vector2 backPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y) + (behindVector * 60f);
        CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(backPos, FLIGHT_PREP_TIME));
        yield return WaitForFrames.WaitWrapper(FLIGHT_PREP_TIME);

        // fly to target
        float clampedX = Mathf.Clamp(targetPos.x, Constant.GAME_BORDER_LEFT, Constant.GAME_BORDER_RIGHT);
        float clampedY = Mathf.Clamp(targetPos.y, Constant.GAME_BORDER_BOTTOM, Constant.GAME_BORDER_TOP);
        CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(clampedX, clampedY), FLIGHT_TIME));
        yield return WaitForFrames.WaitWrapper(FLIGHT_TIME);

        // spawn inner ring as impact effect
        for (int j = 0; j < BRANCHES * 2; j++)
        {
            Vector2 spawnPos = targetPos + UnityEngine.Random.insideUnitCircle * 55f;
            Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, 0f, 0f, EnemyBulletType.BALL2_DARK_RED, 0);
            CoroutineUtil.RunEntityBoundCoroutine(_DespawnStationaryEntity(entity, 60), entity);
        }
    }

    private IEnumerator<float> _DespawnStationaryEntity(Entity entity, int waitDuration)
    {
        yield return WaitForFrames.WaitWrapper(waitDuration);

        ECSEntitySpawner.DespawnEntity(entity);
    }

    private IEnumerator<float> _SpreadDarkFlightOuterRing(Entity entity, int waitDuration)
    {
        yield return WaitForFrames.WaitWrapper(waitDuration);

        ECSEntitySpawner.SetBulletSpeed(entity, 1.4f);
    }
}

