using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class ShapeSpell : AbstractSingle
{
    public int GetHP()
    {
        return 5000;
    }

    public override string GetName()
    {
        return "Art Sign [World-forming Shape]";
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

        int wait = 60;

        Vector2[] pool = new Vector2[]{
            new Vector2(122, -120),
            new Vector2(192, -60),
            new Vector2(262, -120),
        };

        while (true)
        {
            FireEquilateralTriangle(DrawFromPool(ref pool, 3));
            yield return WaitForFrames.WaitWrapper(wait);
            FireSquare(DrawFromPool(ref pool, 2));
            yield return WaitForFrames.WaitWrapper(wait);
            FireCircle(DrawFromPool(ref pool, 1));
            yield return WaitForFrames.WaitWrapper(wait);
            wait = Mathf.Max(20, wait - 1);
        }
    }

    private Vector2 DrawFromPool(ref Vector2[] pool, int choicesLeft = -1)
    {
        if (choicesLeft == -1)
        {
            choicesLeft = pool.Length;
        }
        int roll = Random.Range(0, choicesLeft);
        var result = pool[roll];
        (pool[roll], pool[choicesLeft - 1]) = (pool[choicesLeft - 1], pool[roll]);
        return result;
    }

    private void FireEquilateralTriangle(Vector2 pos)
    {
        const int BULLETS_PER_EDGE = 20;
        const float SPREAD = 120f / (BULLETS_PER_EDGE - 1);
        const float HALF_FAN_SPREAD = 60f;
        float rotation = Random.Range(0f, 360f);
        float baseSpeed = Random.Range(1f, 3.5f);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < BULLETS_PER_EDGE; j++)
            {
                float offsetAngle = -HALF_FAN_SPREAD + SPREAD * j;
                float facing = rotation + offsetAngle;
                float speed = baseSpeed / Mathf.Cos(Mathf.Deg2Rad * offsetAngle);
                ECSEntitySpawner.SpawnEnemyBulletE1(pos, speed, facing, EnemyBulletType.ARROW_RED, 5);
            }
            rotation += 120f;
        }
    }

    private void FireSquare(Vector2 pos)
    {
        const int TOTAL_BULLETS = 60;
        float rotation = Random.Range(0f, 360f);
        const float SPREAD = 360f / TOTAL_BULLETS;
        float baseSpeed = Random.Range(1f, 3.5f);
        for (int i = 0; i < TOTAL_BULLETS; i++)
        {
            float facing = rotation + SPREAD * i;
            float speed = baseSpeed / Mathf.Cos(((facing + 45f) % 90f - 45f) * Mathf.Deg2Rad);
            ECSEntitySpawner.SpawnEnemyBulletE1(pos, speed, facing, EnemyBulletType.ARROW_BLUE, 5);
        }
    }

    private void FireCircle(Vector2 pos)
    {
        const int TOTAL_BULLETS = 60;
        float rotation = Random.Range(0f, 360f);
        const float SPREAD = 360f / TOTAL_BULLETS;
        float baseSpeed = Random.Range(1f, 3.5f);
        for (int i = 0; i < TOTAL_BULLETS; i++)
        {
            float facing = rotation + SPREAD * i;
            ECSEntitySpawner.SpawnEnemyBulletE1(pos, baseSpeed, facing, EnemyBulletType.ARROW_GREEN, 5);
        }
    }
}

