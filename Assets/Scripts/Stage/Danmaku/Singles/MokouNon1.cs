using System.Collections.Generic;
using MEC;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class MokouNon1 : AbstractSingle
{
    public int GetHP()
    {
        return 3000;
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

        float angle = 270f;
        int state = 0;
        int lapCompleted = 0;
        float waveOffset = 0.8f;

        while (true)
        {
            CoroutineUtil.StartSingleLoopCRT(_FireSeiranFan(
                new Vector2(192, -90),
                angle + Random.Range(-10f, 10f),
                6,
                4,
                6,
                7f,
                waveOffset
            ));
            yield return WaitForFrames.WaitWrapper(25);
            angle = (angle + 120f) % 360f;
            state = (state + 1) % 3;
            if (state == 0)
            {
                waveOffset *= -1;
                lapCompleted += 1;
                if (lapCompleted % 4 == 0)
                {
                    if (Player.instance != null)
                    {
                        float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                            Player.instance.transform.position.y - enemy.transform.position.y,
                            Player.instance.transform.position.x - enemy.transform.position.x
                        );
                        for (int i = 0; i < 1; i++)
                        {
                            ECSEntitySpawner.SpawnEnemyBulletE1(enemy.transform.position, 1f - 0.2f * i, angleToPlayer, STG.EnemyBulletType.BUBBLE_DARK_YELLOW, 10);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator<float> _FireSeiranFan(Vector2 at, float facing, int repeatTimes, int incrementCount, int delayBetweenWaves, float spreadBetweenBullet, float waveAngleOffset)
    {
        int fanCount = 1;
        for (int i = 0; i < repeatTimes; i++)
        {
            float halfFanSpread = spreadBetweenBullet * (fanCount - 1) * 0.5f;
            for (int j = 0; j < fanCount; j++)
            {
                // if (j < fanCount * 0.5f) {
                ECSEntitySpawner.SpawnEnemyBulletE1(at.x, at.y, 3f, facing - halfFanSpread + spreadBetweenBullet * j + waveAngleOffset * i, STG.EnemyBulletType.ARROW_SKY, 20);
                // }
                // else {
                //     ECSEntitySpawner.SpawnEnemyBulletE1(at.x, at.y, 3f, facing - halfFanSpread + spreadBetweenBullet * j, STG.EnemyBulletType.ARROW_DARK_BLUE, 20);
                // }
            }
            fanCount += incrementCount;
            yield return WaitForFrames.WaitWrapper(delayBetweenWaves);
        }
    }
}
