using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;

public class Nonspell6 : AbstractSingle
{
    public int GetHP()
    {
        return 6000;
    }

    public override string GetName()
    {
        return "Nonspell 6";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 42;
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

        float[] xPositions = { 30, 111, 192, 273, 354 };
        int i = 0;
        int iVel = 1;
        int wait = 55;
        int count = xPositions.Count();
        while (true)
        {
            GameObject bubbleBullet = EnemyBulletPool.SpawnBulletA1(xPositions[i] + Random.Range(-20f, 20f), Constant.GAME_BORDER_TOP, 2.5f, 270f, EnemyBulletType.BUBBLE_DARK_GREEN, 30);
            if (bubbleBullet.TryGetComponent(out EnemyBullet enemyBullet)) {
                enemyBullet.HitScreenEdgeCallback = Bounce;
            }
            CoroutineUtil.StartSingleLoopCRT(_SpawnFromBubble(bubbleBullet));
            if (i == count - 1)
            {
                iVel = -1;
                wait = Mathf.Max(20, wait - 1);
            }
            else if (i == 0)
            {
                iVel = 1;
                wait = Mathf.Max(20, wait - 1);
            }
            i += iVel;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
        }
    }

    private IEnumerator<float> _SpawnFromBubble(GameObject bubbleBullet)
    {
        yield return WaitForFrames.WaitWrapper(Random.Range(40, 100));
        const int BRANCHES = 3;
        float rot = 360f / BRANCHES;
        while (bubbleBullet.activeInHierarchy)
        {
            float r = Random.Range(-20f, 20f);
            for (int i = 0; i < BRANCHES; i++)
            {
                CoroutineUtil.StartSingleLoopCRT(_AccelerateBullet(EnemyBulletPool.SpawnBulletA1(bubbleBullet, 0f, r + rot * i, EnemyBulletType.AMULET_RED, 30)));
            }
            yield return WaitForFrames.WaitWrapper(Random.Range(100, 200));
        }
    }

    private IEnumerator<float> _AccelerateBullet(GameObject sideBullet)
    {
        yield return WaitForFrames.WaitWrapper(60);

        if (sideBullet.TryGetComponent(out EnemyBullet enemyBullet))
        {
            enemyBullet.speed = 1.2f;
            yield return WaitForFrames.WaitWrapper(15);
            enemyBullet.speed = 0f;
            yield return WaitForFrames.WaitWrapper(60);
            while (sideBullet.activeInHierarchy)
            {
                enemyBullet.speed = Mathf.Min(enemyBullet.speed + 0.1f, 2f);
                yield return Timing.WaitForOneFrame;
            }
        }
    }

    private void Bounce(EnemyBullet enemyBullet)
    {
        if (enemyBullet.transform.position.y <= Constant.GAME_BORDER_BOTTOM)
        {
            enemyBullet.transform.eulerAngles = new Vector3(0f, 0f, 90f);
            (var sprite, var radius, var hitbox, var _1, var _2, var _3) = ShotSheet.GetEnemyBulletData((int)EnemyBulletType.BUBBLE_DARK_YELLOW);
            enemyBullet.SetGraphic(sprite, radius, hitbox);
            enemyBullet.speed *= 2f;
        }
    }
}
