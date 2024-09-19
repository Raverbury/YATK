using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell5 : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
    }

    public override string GetName()
    {
        return "Nonspell 5";
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

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);

        const int BRANCHES = 5;
        const int SUBBRANCHES = 3;
        const float RADIUS = 60f;
        const float SPEED = 2.1f;
        const float SPREAD_ANGLE = 6f;
        int rotation1 = 180;
        int rotation2 = 0;
        CoroutineUtil.StartSingleLoopCRT(_MoveEnemy(enemy));
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(8)));
            for (int i = 0; i < BRANCHES; i++)
            {
                float angle = rotation1 + 360f / BRANCHES * i;
                Vector2 spawnPos = (Vector2)enemy.gameObject.transform.position + new Vector2(RADIUS * Mathf.Cos(Mathf.Deg2Rad * angle), RADIUS * Mathf.Sin(Mathf.Deg2Rad * angle));
                for (int j = 0; j < SUBBRANCHES; j++)
                {
                    float bulletAngle = angle - 75f - ((SUBBRANCHES - 1) / 2 * SPREAD_ANGLE) + j * SPREAD_ANGLE;
                    EnemyBulletPool.SpawnBulletA1(spawnPos, SPEED, bulletAngle, EnemyBulletType.AMULET_BLUE, 10);
                }
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(8)));
            for (int i = 0; i < BRANCHES; i++)
            {
                float angle = rotation2 + 360f / BRANCHES * i;
                Vector2 spawnPos = (Vector2)enemy.gameObject.transform.position + new Vector2(RADIUS * Mathf.Cos(Mathf.Deg2Rad * angle), RADIUS * Mathf.Sin(Mathf.Deg2Rad * angle));
                for (int j = 0; j < SUBBRANCHES; j++)
                {
                    float bulletAngle = angle + 75f - ((SUBBRANCHES - 1) / 2 * SPREAD_ANGLE) + j * SPREAD_ANGLE;
                    EnemyBulletPool.SpawnBulletA1(spawnPos, SPEED, bulletAngle, EnemyBulletType.AMULET_RED, 10);
                }
            }
            rotation1 += 6;
            rotation2 -= 6;
        }
    }

    private IEnumerator<float> _MoveEnemy(Enemy enemy)
    {
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(5 * 60)));
            float targetX = ((Player.instance == null) ? 192f : Player.instance.gameObject.transform.position.x) + Random.Range(-50f, 50f);
            targetX = Mathf.Clamp(targetX, Constant.GAME_BORDER_LEFT + 60, Constant.GAME_BORDER_RIGHT - 60);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(targetX, Random.Range(-60, -120)), 60)));
            enemy.SetAnimState(Enemy.AnimState.Attack);
        }
    }
}
