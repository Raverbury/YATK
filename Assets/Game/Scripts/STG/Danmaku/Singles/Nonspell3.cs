using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class Nonspell3 : AbstractSingle
{
    public int GetHP()
    {
        return 4000;
    }

    public override string GetName()
    {
        return "Nonspell 3";
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

    const int FLOWERS = 40;
    const int FLOWER_PETALS = 5;
    const float FLOWER_RING_RADIUS = 90f;
    const float PETAL_DISTANCE = 8f;

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));

        int state = 0;
        while (true)
        {
            if (state == 0 || state == 4)
            {
                if (state == 4)
                {
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_SpawnFlowerRing(enemy, FLOWERS, 1, EnemyBulletType.RICE_DARK_RED)));
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(102, -100), 90)));
                }
                else
                {
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_SpawnFlowerRing(enemy, FLOWERS, 1, EnemyBulletType.RICE_GREEN)));
                }
            }
            else if (state == 1 || state == 3)
            {
                if (state == 1)
                {
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_SpawnFlowerRing(enemy, FLOWERS, -1, EnemyBulletType.RICE_PURPLE)));
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(282, -100), 90)));
                }
                else
                {
                    yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_SpawnFlowerRing(enemy, FLOWERS, -1, EnemyBulletType.RICE_DARK_YELLOW)));
                }
            }
            else
            {
                enemy.SetAnimState(Enemy.AnimState.Attack);
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(150)));
                Vector2 targetPos = new Vector2(192, -300);
                for (int i = 0; i < 60; i++)
                {
                    Vector2 pos = new Vector2(57, -90);
                    targetPos.x += (state == 2) ? -1 : 1;
                    for (int j = 0; j < 4; j++)
                    {
                        // Vector2 targetPos = (Player.instance == null) ? new Vector2(192, -360) : (Vector2)Player.instance.transform.position;
                        float angle = (j switch
                        {
                            0 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) - 0.05f,
                            1 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) + 0.6f,
                            2 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) - 0.6f,
                            _ => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) + 0.05f,
                        }) * Mathf.Rad2Deg;
                        EnemyBulletPool.SpawnBulletA1(pos, 9, angle, STG.EnemyBulletType.ARROW_SKY, 30);
                        pos.x += 90;
                    }
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(2)));
                }
                yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(192, -90), 90)));
            }
            state = 5 == state ? 0 : state + 1;
        }
    }

    private IEnumerator<float> _SpawnFlowerRing(Enemy enemy, int flowers, int direction, EnemyBulletType enemyBulletType)
    {
        float rotation = 90f + direction * 90f;
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(10)));
        Vector2 enemyCenter = new(192, -90);
        float r = Random.Range(0f, 360f);
        for (int i = 0; i < flowers; i++)
        {
            rotation += direction * 360f / flowers;
            float rotationR = Mathf.Deg2Rad * rotation;
            Vector2 flowerPos = enemyCenter + new Vector2(FLOWER_RING_RADIUS * Mathf.Cos(rotationR), FLOWER_RING_RADIUS * Mathf.Sin(rotationR));
            for (int j = 0; j < FLOWER_PETALS; j++)
            {
                float angle = rotation + 360f / FLOWER_PETALS * j + r;
                float angleR = Mathf.Deg2Rad * angle;
                Vector2 spawnPos = flowerPos + new Vector2(PETAL_DISTANCE * Mathf.Cos(angleR), PETAL_DISTANCE * Mathf.Sin(angleR));
                GameObject bullet = EnemyBulletPool.SpawnBulletA1(spawnPos, 0, angle + 180f, enemyBulletType, 10);
                CoroutineUtil.StartSingleLoopCRT(_ManipulateFlower(bullet));
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(2)));
        }
    }

    private IEnumerator<float> _ManipulateFlower(GameObject gameObject)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(120)));
        if (gameObject.activeInHierarchy)
        {
            gameObject.GetComponent<EnemyBullet>().speed = 2.7f;
        }
    }
}
