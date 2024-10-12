using System.Collections.Generic;
using MEC;
using UnityEngine;

public class Nonspell2 : AbstractSingle
{
    public int GetHP()
    {
        return 1500;
    }

    public override string GetName()
    {
        return "Nonspell 2";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 35;
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

        const int BRANCHES = 3;
        const float SPEED = 3;
        int rotation = 90;
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(10)));
            for (int i = 0; i < BRANCHES; i++)
            {
                float angle = 360f / BRANCHES;
                for (int j = 0; j < BRANCHES; j++)
                {
                    float modSpeed = SPEED * (1 - 0.2f * j);
                    // int delay = 30 * (BRANCHES - j);
                    int delay = 30;
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, modSpeed, angle * i + rotation, STG.EnemyBulletType.AMULET_BLUE, delay);
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, modSpeed, angle * i + 23 + rotation, STG.EnemyBulletType.AMULET_RED, delay);
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, modSpeed, angle * i - 23 + rotation, STG.EnemyBulletType.AMULET_PURPLE, delay);
                }
            }
            rotation -= 7;
        }
    }
}
