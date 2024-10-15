using System.Collections.Generic;
using MEC;
using STG;
using Unity.Entities;
using UnityEngine;

public class StarSpell1 : AbstractSingle
{
    public int GetHP()
    {
        return 7000;
    }

    public override string GetName()
    {
        return "Miracle [Spreading Hope]";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 50;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -120), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        float facingAngle = 90;

        while (true)
        {
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_SpawnStarShape(
                enemy.transform.position,
                facingAngle,
                200,
                13,
                1
            )));
            yield return WaitForFrames.WaitWrapper(180);
            facingAngle = Random.Range(0f, 420f);
        }
    }

    IEnumerator<float> _SpawnStarShape(Vector2 centerPos, float facing, float edgeLength, int bulletsPerEdge, int spawnInterval)
    {
        const int TIME_TO_BLOOM = 60;

        float centerToTipLength = (edgeLength / 2) / Mathf.Cos(Mathf.Deg2Rad * 18f);
        int totalNumOfBullets = bulletsPerEdge * 5;
        int remainingBullets = totalNumOfBullets;

        float distanceBetweenBulletOnSameEdge = edgeLength / (bulletsPerEdge - 1);

        float r1 = Random.Range(0f, 360f);
        float r2 = Random.Range(0f, 360f);

        for (int i = 0; i < 5; i++)
        {
            float centerToTipAngle = (facing - 144f * i) % 360f;
            float tipX = centerPos.x + Mathf.Cos(Mathf.Deg2Rad * centerToTipAngle) * centerToTipLength;
            float tipY = centerPos.y + Mathf.Sin(Mathf.Deg2Rad * centerToTipAngle) * centerToTipLength;
            float edgeRunAngle = (centerToTipAngle - 162f) % 360f;
            for (int j = 0; j < bulletsPerEdge; j++)
            {
                float distanceFromTipToBullet = distanceBetweenBulletOnSameEdge * j;
                float bulletX = tipX + Mathf.Cos(Mathf.Deg2Rad * edgeRunAngle) * distanceFromTipToBullet;
                float bulletY = tipY + Mathf.Sin(Mathf.Deg2Rad * edgeRunAngle) * distanceFromTipToBullet;
                float bulletAngle = edgeRunAngle - 90 - (j - (bulletsPerEdge - 1) / 2f) * 5;
                float bulletAngle2 = edgeRunAngle + 90 + 7 * j + r1;
                float bulletAngle3 = edgeRunAngle + 90 + (j - (bulletsPerEdge - 1) / 2f) * 7;
                float bulletAngle4 = edgeRunAngle + 2.3f * j + r2;
                int waitTime = TIME_TO_BLOOM + remainingBullets;
                Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0, bulletAngle, EnemyBulletType.BALL2_LIME, 1);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity, 1.2f, waitTime), entity);
                Entity entity2 = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0, bulletAngle2, EnemyBulletType.BALL2_PURPLE, 1);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity2, 1.4f, waitTime), entity2);
                Entity entity3 = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0, bulletAngle3, EnemyBulletType.BALL2_SKY, 1);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity3, 1.2f, waitTime), entity3);
                Entity entity4 = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0, bulletAngle4, EnemyBulletType.BALL2_ORANGE, 1);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity4, 0.9f + (bulletsPerEdge - j) * 0.06f, waitTime), entity4);
                remainingBullets -= 1;
                yield return WaitForFrames.WaitWrapper(spawnInterval);
            }
        }
    }

    IEnumerator<float> _Manipulate(Entity entity, float speed, int wait)
    {
        yield return WaitForFrames.WaitWrapper(wait);

        ECSEntitySpawner.SetBulletSpeed(entity, speed);
    }
}
