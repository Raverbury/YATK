using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class Pattern01 : AbstractSingle
{
    public int GetHP()
    {
        return 1600;
    }

    public override string GetName()
    {
        return "Nonspell 1";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 32;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);

        const int SEGMENTS = 8;
        const int BRANCHES = SEGMENTS * 11;
        int rotation = 90;
        bool oddWave = true;
        while (true)
        {
            float r = Random.Range(0, 10);
            for (int i = 0; i < BRANCHES; i++)
            {
                // GameObject bullet = pool.SpawnBulletA1(192, -60, 2 + 3 * ((-1 * Mathf.Abs(i - BRANCHES / 2)) + BRANCHES / 2) / (BRANCHES / 2), 360f / (BRANCHES / 2) * i + r, 0.5f);
                int j = (i % SEGMENTS) switch
                {
                    0 or 7 => 1,
                    1 or 6 => 3,
                    2 or 5 => 5,
                    _ => 10,
                };
                float speed = 2 + 0.5f * j;
                float angle = 360f / BRANCHES * i + rotation;
                EnemyBulletType color = oddWave ? EnemyBulletType.ARROW_DARK_BLUE : EnemyBulletType.ARROW_DARK_GREEN;
                GameObject bullet = EnemyBulletPool.SpawnBulletA1(enemy.gameObject, speed, angle, color, 30);
                CoroutineUtil.StartSingleLoopCRT(_Manipulate(bullet));
            }
            rotation += 7;
            oddWave = !oddWave;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(45)));
        }
    }

    IEnumerator<float> _Manipulate(GameObject bullet)
    {
        for (int __delay = 0; __delay < 15; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        while (bullet.activeInHierarchy && Vector2.Distance(bullet.transform.position, new Vector2(192, -60)) < 260)
        {
            yield return Timing.WaitForOneFrame;
        }

        enemyBullet.speed = 1.6f;
    }
}
