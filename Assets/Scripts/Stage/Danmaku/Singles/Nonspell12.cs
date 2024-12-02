using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell12 : AbstractSingle
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
        return 60;
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

        // const int BRANCHES = 78;
        const float RADIUS = 80f;
        float rotation = 0f;
        // const float spread = 360f / BRANCHES;
        float spread = 5.7f;
        int counter = 0;
        int batchCount = 7;
        int loopCount = 0;
        while (true)
        {
            float spawnX = enemy.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * rotation) * RADIUS;
            float spawnY = enemy.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * rotation) * RADIUS;
            ECSEntitySpawner.SpawnEnemyBulletE1(spawnX, spawnY, 3f, rotation, EnemyBulletType.AMULET_DARK_BLUE, 10 - counter);
            ECSEntitySpawner.SpawnEnemyBulletE1(spawnX, spawnY, 3f, rotation + 180f, EnemyBulletType.AMULET_DARK_RED, 14 - counter);
            // ECSEntitySpawner.SpawnEnemyBulletE1(spawnX, spawnY, 3f, facing, EnemyBulletType.AMULET_DARK_BLUE, 10 - counter);
            rotation = (rotation + spread) % 360f;
            counter = (counter + 1) % batchCount;
            yield return Timing.WaitForOneFrame;
            loopCount = (loopCount + 1) % 200;
            if (loopCount == 0)
            {
                batchCount = Mathf.Max(3, batchCount - 1);
                spread = Mathf.Max(4.53f, spread * 0.9f);
            }
        }
    }
}

