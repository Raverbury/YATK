using System.Collections.Generic;
using MEC;
using STG;
using Unity.Entities;
using UnityEngine;

public class SurroundSpell1 : AbstractSingle
{
    public int GetHP()
    {
        return 10000;
    }

    public override string GetName()
    {
        return "Miracle [Halley's Comet]";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 80;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -120), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        Vector2 targetPos = new Vector2(192f, -360f);
        int dir = 1;
        int wait = 120;

        while (true)
        {
            if (Player.instance != null)
            {
                targetPos = Player.instance.transform.position;
            }
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_DoSurround(
                targetPos,
                dir
            )));
            yield return WaitForFrames.WaitWrapper(320 + wait);
            dir *= -1;
            wait = Mathf.Max(0, wait - 10);
        }
    }

    IEnumerator<float> _DoSurround(Vector2 targetPos, int dir)
    {
        targetPos.y = Mathf.Clamp(targetPos.y, Constant.GAME_BORDER_BOTTOM + 60, Constant.GAME_BORDER_TOP - 60);
        // spawn init ring
        const float OUTER_RADIUS = 120f;
        const int OUTER_COUNT = 80;
        const float OUTER_SPREAD = 360f / OUTER_COUNT;
        for (int i = 0; i < OUTER_COUNT * 2; i++)
        {
            float facing = 90f + 90f * dir - OUTER_SPREAD * i * dir;
            float bulletX = targetPos.x + Mathf.Cos(facing * Mathf.Deg2Rad) * OUTER_RADIUS;
            float bulletY = targetPos.y + Mathf.Sin(facing * Mathf.Deg2Rad) * OUTER_RADIUS;
            Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0.03f, facing - 180, EnemyBulletType.BALL2_DARK_YELLOW, 5, false);
            CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity, 2f, 420 + i), entity);
        }
        yield return WaitForFrames.WaitWrapper(90);

        // spawn circle closing in
        const float INNER_RADIUS = 100f;
        const int INNER_COUNT = 80;
        const float INNER_SPREAD = 360f / OUTER_COUNT;
        for (int i = 0; i < INNER_COUNT * 2; i++)
        {
            float facing = 90f - 90f * dir + INNER_SPREAD * i * dir;
            float bulletX = targetPos.x + Mathf.Cos(facing * Mathf.Deg2Rad) * INNER_RADIUS;
            float bulletY = targetPos.y + Mathf.Sin(facing * Mathf.Deg2Rad) * INNER_RADIUS;
            Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0.05f, facing - 180f, EnemyBulletType.RICE_DARK_GREEN, 5, false);
            CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity, 1.5f, 120), entity);
            yield return Timing.WaitForOneFrame;
        }
    }


    IEnumerator<float> _Manipulate(Entity entity, float speed, int wait)
    {
        yield return WaitForFrames.WaitWrapper(wait);

        ECSEntitySpawner.SetBulletSpeed(entity, speed);
    }
}
