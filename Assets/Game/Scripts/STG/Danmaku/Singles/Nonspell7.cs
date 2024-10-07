using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;

public class Nonspell7 : AbstractSingle
{
    public int GetHP()
    {
        return 6800;
    }

    public override string GetName()
    {
        return "Nonspell 7";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 47;
    }

    public override bool IsTimeout()
    {
        return true;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        yield return WaitForFrames.WaitWrapper(30);
        AbstractSingle.PatternStart?.Invoke();
        yield return WaitForFrames.WaitWrapper(30);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, 200), 60)));
        // yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);

        float[] xPositions = { 48, 144, 240, 336 };
        EnemyBulletType[] bulletTypes = {
            EnemyBulletType.BUBBLE_DARK_YELLOW,
            EnemyBulletType.BUBBLE_DARK_GREEN,
            EnemyBulletType.BUBBLE_DARK_BLUE,
            EnemyBulletType.BUBBLE_DARK_RED
        };
        int wait = 110;
        int BURSTS = 4;
        int BRANCHES = 3;
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
                        CoroutineUtil.StartSingleLoopCRT(_InitialSpawnMotion(EnemyBulletPool.SpawnBulletA1(xPositions[i], -330f + 20 * j, 6, 90, bulletTypes[i], 10), branchRotation * k));
                    }
                    yield return WaitForFrames.WaitWrapper(wait / BURSTS);
                }
                yield return WaitForFrames.WaitWrapper(wait / 3);
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
            wait = Mathf.Max(30, wait - 15);
            BURSTS = Mathf.Min(7, BURSTS + 1);
            BRANCHES = Mathf.Min(15, BRANCHES + 2);
        }
    }

    private IEnumerator<float> _InitialSpawnMotion(GameObject bulletGameObject, float rotOffset)
    {
        if (bulletGameObject.TryGetComponent(out EnemyBullet enemyBullet))
        {
            float slowDown = enemyBullet.speed / 60;
            for (int i = 0; i < 60; i++)
            {
                enemyBullet.speed -= slowDown;
                yield return Timing.WaitForOneFrame;
            }
            enemyBullet.speed = 0f;
            yield return WaitForFrames.WaitWrapper(60);
            if (Player.instance != null)
            {
                enemyBullet.transform.eulerAngles = new Vector3(0f, 0f, rotOffset + Mathf.Rad2Deg * Mathf.Atan2(
                    Player.instance.transform.position.y - enemyBullet.transform.position.y,
                    Player.instance.transform.position.x - enemyBullet.transform.position.x
                ));
                // enemyBullet.transform.right = enemyBullet.transform.position - Player.instance.gameObject.transform.position;
                // enemyBullet.transform.eulerAngles = new Vector3(0f, 0f, 270f);
                for (int i = 0; i < 20; i++)
                {
                    enemyBullet.speed += 0.15f;
                    yield return Timing.WaitForOneFrame;
                }
            }
        }
    }
}
