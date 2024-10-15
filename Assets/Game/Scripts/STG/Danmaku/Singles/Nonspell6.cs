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
        return "Jade Sign [Heaven's Intervention]";
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
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
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
            CoroutineUtil.RunEntityBoundCoroutine(_SpawnFromBubble(entity), entity);
            if (i == count - 1)
            {
                iVel = -1;
                wait = Mathf.Max(10, wait - 2);
            }
            else if (i == 0)
            {
                iVel = 1;
                wait = Mathf.Max(10, wait - 2);
            }
            i += iVel;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
        }
    }

    private IEnumerator<float> _SpawnFromBubble(Entity bubbleBulletEntity)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return WaitForFrames.WaitWrapper(Random.Range(30, 50));
        int branches = Random.Range(2, 8);
        float rot = 360f / branches;
        while (!ECSEntitySpawner.EntityIsDisabled(bubbleBulletEntity))
        {
            float r = Random.Range(-20f, 20f);
            for (int i = 0; i < branches; i++)
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bubbleBulletEntity);
                Entity subBulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletTransform.Position.x, bulletTransform.Position.y, 0f, r + rot * i, EnemyBulletType.AMULET_RED, 30);
                CoroutineUtil.RunEntityBoundCoroutine(_AccelerateBullet(subBulletEntity), subBulletEntity);
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
