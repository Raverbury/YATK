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
        return 40;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    const int FLOWERS = 40;
    const int FLOWER_PETALS = 5;
    const float FLOWER_RING_RADIUS = 80f;

    protected override IEnumerator<float> _Loop()
    {
        StageManager.SpawnNamedEnemy(out GameObject enemyGameObject, -100, 100, "mokou");
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.SetEmptyHpCircle();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));

        int state = 0;
        while (!enemy.IsDead())
        {
            if (state == 0 || state == 4)
            {
                if (state == 4)
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(_SpawnFlowerRing(enemy, FLOWERS, 1, EnemyBulletType.RICE_YELLOW)));
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(102, -100), 60)));
                }
                else
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(_SpawnFlowerRing(enemy, FLOWERS, 1, EnemyBulletType.RICE_GREEN)));
                }
            }
            else if (state == 1 || state == 3)
            {
                if (state == 1)
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(_SpawnFlowerRing(enemy, FLOWERS, -1, EnemyBulletType.RICE_PURPLE)));
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(282, -100), 60)));
                }
                else
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(_SpawnFlowerRing(enemy, FLOWERS, -1, EnemyBulletType.RICE_ORANGE)));
                }
            }
            else
            {
                enemy.SetAnimState(Enemy.AnimState.Attack);
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(150)));
                for (int i = 0; i < 60; i++)
                {
                    Vector2 pos = new Vector2(57, -90);
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 targetPos = (Player.instance == null) ? new Vector2(192, -360) : (Vector2)Player.instance.transform.position;
                        float angle = (j switch
                        {
                            0 or 3 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x),
                            1 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) + 0.3f,
                            2 => Mathf.Atan2(targetPos.y - pos.y, targetPos.x - pos.x) - 0.3f,
                            _ => 180,
                        }) * Mathf.Rad2Deg - 90;
                        StageManager.SpawnBulletA1(pos, 9, angle, STG.EnemyBulletType.ARROW_SKY, 30);
                        pos.x += 90;
                    }
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(2)));
                }
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
            }
            state = 5 == state ? 0 : state + 1;
        }
        enemy.SetAnimState(Enemy.AnimState.Idle);
    }

    private IEnumerator<float> _SpawnFlowerRing(Enemy enemy, int flowers, int direction, EnemyBulletType enemyBulletType)
    {
        float rotation = 90f + direction * 90f;
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(10)));
        Vector2 enemyCenter = new(192, -90);
        for (int i = 0; i < flowers; i++)
        {
            rotation += direction * 360f / flowers;
            float rotationR = Mathf.Deg2Rad * rotation;
            Vector2 flowerPos = enemyCenter + new Vector2(FLOWER_RING_RADIUS * Mathf.Cos(rotationR), FLOWER_RING_RADIUS * Mathf.Sin(rotationR));
            for (int j = 0; j < FLOWER_PETALS; j++)
            {
                float angle = 360f / FLOWER_PETALS * j;
                float angleR = Mathf.Deg2Rad * angle;
                Vector2 spawnPos = flowerPos + new Vector2(10 * Mathf.Cos(angleR), 10 * Mathf.Sin(angleR));
                GameObject bullet = StageManager.SpawnBulletA1(spawnPos, 0, -rotation + angle - 90f, enemyBulletType, 10);
                Timing.RunCoroutine(_ManipulateFlower(bullet));
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(2)));
        }
    }

    private IEnumerator<float> _ManipulateFlower(GameObject gameObject)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(150)));
        gameObject.GetComponent<EnemyBullet>().speed = 3f;
    }
}
