using System.Collections.Generic;
using System.Diagnostics;
using MEC;
using STG;
using UnityEngine;

public class Nonspell2 : AbstractSingle
{
    public int GetHP()
    {
        return 1500;
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
        const int BURSTS = 5;
        const float SPEED = 3;
        int dir = 17;
        int rotation = 90;
        float angle = 360f / BRANCHES;
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(25)));
            // SFXPlayer.RequestPlayTan1Sound?.Invoke();
            for (int i = 0; i < BURSTS; i++)
            {
                EnemyBulletType enemyBulletType = i switch
                {
                    0 => EnemyBulletType.AMULET_RED,
                    1 => EnemyBulletType.AMULET_SKY,
                    2 => EnemyBulletType.AMULET_GREEN,
                    3 => EnemyBulletType.AMULET_ORANGE,
                    _ => EnemyBulletType.AMULET_YELLOW,
                };
                for (int j = 0; j < BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, SPEED * (1 - i * 0.1f), angle * j + rotation + i * dir, enemyBulletType, 0);
                }
            }
            if (Random.Range(0f, 1f) < 0.5f)
            {
                dir *= -1;
            }
            rotation = Random.Range(0, 361);
        }
    }
}
