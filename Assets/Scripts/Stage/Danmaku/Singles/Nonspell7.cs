using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;

public class Nonspell7 : AbstractSingle
{
    public int GetHP()
    {
        return 6800;
    }

    public override string GetName()
    {
        return "Tetra Assault";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 60;
    }

    protected override bool IsTimeout()
    {
        return true;
    }

    protected override bool IsSpellCard()
    {
        return true;
    }

    protected override bool IsBossAttack()
    {
        return true;
    }

    protected override LoopableBGM SingleBGM()
    {
        return RuntimeGameData.Registry.BGM_08_MOKOU;
    }

    protected override void CleanUp()
    {
        if (enemy)
        {
            enemy.SetEmptyHpCircle();
        }
        for (int i = 0; i < 20; i++)
        {
            ECSEntitySpawner.SpawnItemI1(new Vector2(192f, -80f), ItemType.POWER_ITEM);
        }
    }

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        bool res = enemy && enemy.IsDead();
        if (res)
        {
            enemy.SetEmptyHpCircle();
        }
        return res;
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new());
        yield return WaitForFrames.WaitWrapper(30);
        yield return WaitForFrames.WaitWrapper(30);
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, 100), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        // yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        StageManager.DestroyNamedEnemy("mokou");

        float[] xPositions = { 48, 144, 240, 336 };
        EnemyBulletType[] bulletTypes = {
            EnemyBulletType.BUBBLE_DARK_YELLOW,
            EnemyBulletType.BUBBLE_DARK_GREEN,
            EnemyBulletType.BUBBLE_DARK_BLUE,
            EnemyBulletType.BUBBLE_DARK_RED
        };
        int wait = 110;
        int BURSTS = 4;
        int BRANCHES = 2;
        // int i = 0;
        int count = xPositions.Count();
        while (true)
        {
            float branchRotation = 360f / BRANCHES;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < BURSTS; j++)
                {
                    for (int k = 0; k < BRANCHES; k++)
                    {
                        Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i], -330f + 20 * j, 6, 90, bulletTypes[i], 10);
                        CoroutineUtil.RunEntityBoundCoroutine(_InitialSpawnMotion(entity, branchRotation * k), entity);
                    }
                    yield return WaitForFrames.WaitWrapper(wait / BURSTS);
                }
                yield return WaitForFrames.WaitWrapper(wait / 3);
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
            wait = Mathf.Max(10, wait - 15);
            BURSTS = Mathf.Min(7, BURSTS + 1);
            BRANCHES = Mathf.Min(8, BRANCHES + 1);
        }
    }

    private IEnumerator<float> _InitialSpawnMotion(Entity bulletEntity, float rotOffset)
    {
        yield return WaitForFrames.WaitWrapper(10);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        float speed = 6f;
        float slowDown = speed / 60f;
        for (int i = 0; i < 60; i++)
        {
            speed -= slowDown;
            ECSEntitySpawner.SetBulletSpeed(bulletEntity, speed);
            yield return Timing.WaitForOneFrame;
        }
        ECSEntitySpawner.SetBulletSpeed(bulletEntity, 0f);
        yield return WaitForFrames.WaitWrapper(60);
        if (Player.instance != null)
        {
            LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
            ECSEntitySpawner.SetBulletFacing(bulletEntity, rotOffset + Mathf.Rad2Deg * Mathf.Atan2(
                Player.instance.transform.position.y - bulletTransform.Position.y,
                Player.instance.transform.position.x - bulletTransform.Position.x
            ));
            // enemyBullet.transform.right = enemyBullet.transform.position - Player.instance.gameObject.transform.position;
            // enemyBullet.transform.eulerAngles = new Vector3(0f, 0f, 270f);
            for (int i = 0; i < 20; i++)
            {
                speed += 0.15f;
                ECSEntitySpawner.SetBulletSpeed(bulletEntity, speed);
                yield return Timing.WaitForOneFrame;
            }
        }

    }
}
