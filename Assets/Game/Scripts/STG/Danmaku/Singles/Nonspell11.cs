using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell11 : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
    }

    public override string GetName()
    {
        return "Nonspell 10";
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
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        const int BURSTS = 7;
        const int BRANCHES = 20;
        const float GAP = 3f;
        // int i = 0;
        float branchRotation = 360f / BRANCHES;
        int state = 0;
        // CoroutineUtil.StartSingleLoopCRT(_MoveEnemy(enemy));
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
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.transform.position, 3.5f + 0.3f * j, GAP * (state % 2 == 0 ? j : -j) + angleToPlayer + branchRotation * i, j switch
                    {
                        0 or 6 or 3 => EnemyBulletType.AMULET_RED,
                        1 or 5 => EnemyBulletType.AMULET_PURPLE,
                        _ => EnemyBulletType.AMULET_BLUE,
                        // 3 => EnemyBulletType.AMULET_SKY,
                        // 4 => EnemyBulletType.AMULET_GREEN,
                        // 5 => EnemyBulletType.AMULET_YELLOW,
                        // _ => EnemyBulletType.AMULET_ORANGE,
                    }, 10);
                }
            }
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(40)));
            if (state == 2 || state == 5)
            {
                float targetX = ((Player.instance == null) ? 192f : Player.instance.gameObject.transform.position.x) + Random.Range(-20f, 20f);
                targetX = Mathf.Clamp(targetX, Constant.GAME_BORDER_LEFT + 60, Constant.GAME_BORDER_RIGHT - 60);
                yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(targetX, Random.Range(-90, -60)), 60)));
                enemy.SetAnimState(Enemy.AnimState.Attack);
                yield return WaitForFrames.WaitWrapper(40);
            }
            state = (state + 1) % 6;
        }
    }
}

