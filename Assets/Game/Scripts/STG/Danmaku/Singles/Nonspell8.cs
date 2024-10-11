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
        Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        const int BURSTS = 2;
        const int BRANCHES = 70;
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
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, 1.5f + 5f * j, angleToPlayer + branchRotation * i, EnemyBulletType.RICE_SKY, 10);
                }
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(80)));
        }
    }

    private IEnumerator<float> _MoveEnemy(Enemy enemy)
    {
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(90)));
            float targetX = ((Player.instance == null) ? 192f : Player.instance.gameObject.transform.position.x) + Random.Range(-20f, 20f);
            targetX = Mathf.Clamp(targetX, Constant.GAME_BORDER_LEFT + 60, Constant.GAME_BORDER_RIGHT - 60);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(targetX, Random.Range(-60, -60)), 60)));
            enemy.SetAnimState(Enemy.AnimState.Attack);
        }
    }
}

