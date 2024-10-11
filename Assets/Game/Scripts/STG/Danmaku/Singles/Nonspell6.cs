using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;

public class Nonspell6 : AbstractSingle
{
    public int GetHP()
    {
        return 6000;
    }

    public override string GetName()
    {
        return "Nonspell 6";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 42;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        float[] xPositions = { 30, 111, 192, 273, 354 };
        int i = 0;
        int iVel = 1;
        int wait = 55;
        int count = xPositions.Count();
        while (true)
        {
            Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + Random.Range(-20f, 20f), Constant.GAME_BORDER_TOP, 2.5f, 270f, EnemyBulletType.BUBBLE_DARK_GREEN, 30);
            // if (bubbleBulletEntity.TryGetComponent(out EnemyBullet enemyBullet)) {
            //     enemyBullet.HitScreenEdgeCallback = Bounce;
            // }
            CoroutineUtil.StartSingleLoopCRT(_SpawnFromBubble(entity));
            if (i == count - 1)
            {
                iVel = -1;
                wait = Mathf.Max(20, wait - 1);
            }
            else if (i == 0)
            {
                iVel = 1;
                wait = Mathf.Max(20, wait - 1);
            }
            i += iVel;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
        }
    }

    private IEnumerator<float> _SpawnFromBubble(Entity bubbleBulletEntity)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return WaitForFrames.WaitWrapper(Random.Range(40, 100));
        const int BRANCHES = 3;
        float rot = 360f / BRANCHES;
        while (!ECSEntitySpawner.EntityIsDisabled(bubbleBulletEntity))
        {
            float r = Random.Range(-20f, 20f);
            for (int i = 0; i < BRANCHES; i++)
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bubbleBulletEntity);
                CoroutineUtil.StartSingleLoopCRT(_AccelerateBullet(ECSEntitySpawner.SpawnEnemyBulletE1(bulletTransform.Position.x, bulletTransform.Position.y, 0f, r + rot * i, EnemyBulletType.AMULET_RED, 30)));
            }
            yield return WaitForFrames.WaitWrapper(Random.Range(100, 200));
        }
    }

    private IEnumerator<float> _AccelerateBullet(Entity subBulletEntity)
    {
        yield return WaitForFrames.WaitWrapper(60);

        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 1.2f);
        yield return WaitForFrames.WaitWrapper(15);
        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 0f);
        yield return WaitForFrames.WaitWrapper(60);
        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 0f);
        float speed = 0f;
        while (!ECSEntitySpawner.EntityIsDisabled(subBulletEntity))
        {
            speed = Mathf.Min(speed + 0.1f, 2f);
            ECSEntitySpawner.SetBulletSpeed(subBulletEntity, speed);
            yield return Timing.WaitForOneFrame;
        }
    }

    // private void Bounce(EnemyBullet enemyBullet)
    // {
    //     if (enemyBullet.transform.position.y <= Constant.GAME_BORDER_BOTTOM)
    //     {
    //         enemyBullet.transform.eulerAngles = new Vector3(0f, 0f, 90f);
    //         (var sprite, var radius, var hitbox, var _1, var _2, var _3) = ShotSheet.GetEnemyBulletData((int)EnemyBulletType.BUBBLE_DARK_YELLOW);
    //         enemyBullet.SetGraphic(sprite, radius, hitbox);
    //         enemyBullet.speed *= 2f;
    //     }
    // }
}
