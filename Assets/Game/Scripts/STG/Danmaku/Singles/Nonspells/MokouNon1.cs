using System.Collections.Generic;
using MEC;
using UnityEngine;

public class MokouNon1 : AbstractSingle
{
    public int GetHP()
    {
        return 3000;
    }

    public override string GetName()
    {
        return "Mokou Non 1";
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
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);

        while (true)
        {
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(_FireSeiranFan(
                new Vector2(192, -90),
                270f,
                10,
                1,
                3,
                3f
            )));
            yield return WaitForFrames.WaitWrapper(100);
        }
    }

    private IEnumerator<float> _FireSeiranFan(Vector2 at, float facing, int repeatTimes, int incrementCount, int delayBetweenWaves, float spreadBetweenBullet)
    {
        int fanCount = 1;
        for (int i = 0; i < repeatTimes; i++)
        {
            float halfFanSpread = spreadBetweenBullet * (fanCount - 1) * 0.5f;
            for (int j = 0; j < fanCount; j++)
            {
                if (j < fanCount * 0.5f) {
                    ECSBulletSpawner.SpawnBulletA1(at.x, at.y, 3f, facing - halfFanSpread + spreadBetweenBullet * j, STG.EnemyBulletType.ARROW_DARK_BLUE, 20);
                }
                else {
                    EnemyBulletPool.SpawnBulletA1(at.x, at.y, 3f, facing - halfFanSpread + spreadBetweenBullet * j, STG.EnemyBulletType.ARROW_DARK_BLUE, 20);
                }
            }
            fanCount += incrementCount;
            yield return WaitForFrames.WaitWrapper(delayBetweenWaves);
        }
    }
}
