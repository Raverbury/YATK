using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell13 : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
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

        const int BRANCHES = 50;
        const float SPREAD = 360f / BRANCHES;
        Vector2 leftSpawn = new Vector2(132f, -120f);
        Vector2 rightSpawn = new Vector2(252f, -120f);
        Vector2 spawnPos = Vector2.zero;
        float speed = 1f;
        float r = Random.Range(0f, 360f);
        while (true)
        {
            r = Random.Range(0f, 360f);
            spawnPos = leftSpawn + Random.insideUnitCircle * 60f;
            speed = Random.Range(1.8f, 3f);
            for (int i = 0; i < BRANCHES; i++)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, speed, r + SPREAD * i, EnemyBulletType.AMULET_RED, 5);
            }
            yield return WaitForFrames.WaitWrapper(45);
            r = Random.Range(0f, 360f);
            spawnPos = rightSpawn + Random.insideUnitCircle * 60f;
            speed = Random.Range(1.8f, 3f);
            for (int i = 0; i < BRANCHES; i++)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, speed, r + SPREAD * i, EnemyBulletType.AMULET_PURPLE, 5);
            }
            yield return WaitForFrames.WaitWrapper(45);
        }
    }
}

