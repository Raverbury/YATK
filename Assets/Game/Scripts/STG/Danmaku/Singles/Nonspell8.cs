using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;

public class Nonspell8 : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
    }

    public override string GetName()
    {
        return "Nonspell 8";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 45;
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

        const int BURSTS = 2;
        const int BRANCHES = 60;
        // int i = 0;
        float branchRotation = 360f / BRANCHES;
        CoroutineUtil.StartSingleLoopCRT(_MoveEnemy(enemy));
        while (true)
        {
            float angleToPlayer = 270f;
            if (Player.instance != null)
            {
                angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                    enemy.transform.position.y - Player.instance.transform.position.y,
                    enemy.transform.position.x - Player.instance.transform.position.x
                );
            }
            for (int i = 0; i < BRANCHES; i++)
            {
                for (int j = 0; j < BURSTS; j++)
                {
                    EnemyBulletPool.SpawnBulletA1(enemy.gameObject, 3.5f + 2f * j, angleToPlayer + branchRotation * i, EnemyBulletType.RICE_SKY, 10);
                }
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(80)));
        }
    }

    private IEnumerator<float> _MoveEnemy(Enemy enemy)
    {
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(5 * 60)));
            float targetX = ((Player.instance == null) ? 192f : Player.instance.gameObject.transform.position.x) + Random.Range(-20f, 20f);
            targetX = Mathf.Clamp(targetX, Constant.GAME_BORDER_LEFT + 60, Constant.GAME_BORDER_RIGHT - 60);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(targetX, Random.Range(-60, -60)), 60)));
            enemy.SetAnimState(Enemy.AnimState.Attack);
        }
    }
}

