using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using Unity.Entities;
using Assets.Scripts.Util;

public class YoumuNon : AbstractSingle
{
    public int GetHP()
    {
        return 23000;
    }

    public override string GetName()
    {
        return "Formless Sword [Subspace Convergence]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 49;
    }

    protected override bool IsTimeout()
    {
        return false;
    }

    protected override bool IsSpellCard()
    {
        return true;
    }

    protected override bool IsBossAttack()
    {
        return true;
    }

    protected override LoopableBGM SingleBGM()
    {
        return RuntimeGameData.Registry.BGM_08_MOKOU;
    }

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        return enemy && enemy.IsDead();
    }

    protected override void CleanUp()
    {
        if (enemy)
        {
            enemy.SetEmptyHpCircle();
        }
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.POWER_ITEM, 7),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -160), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(
                _SpawnYoumuNon1(
                    enemy.transform.position,
                    Player.instance ? Player.instance.transform.position : new Vector3(192f, -360f),
                    30,
                    EnemyBulletType.BALL2_YELLOW,
                    EnemyBulletType.BALL2_GREEN,
                    7
                )
            ));
            yield return WaitForFrames.WaitWrapper(200);
        }
    }

    private IEnumerator<float> _SpawnYoumuNon1(Vector2 centerPos, Vector2 aimPos, int repeatTimes, EnemyBulletType enemyBulletType1, EnemyBulletType enemyBulletType2, int delayBetweenWaves)
    {
        float maxDistance = 80f;
        float distanceShrink = 160f / repeatTimes;
        float spread = 15f;
        float aimAngle = new Vector3(centerPos.x, centerPos.y).AngleTo(aimPos);
        // float flucProgress = 0;
        for (int i = 1; i < repeatTimes + 1; i++)
        {
            int bullet = Mathf.Min(i, i);
            float distance = maxDistance - distanceShrink * i;
            // flucProgress += (float)i / MathUtil.Sum(1, repeatTimes);
            Vector2 tipPos = centerPos.ExtendBy(distance, aimAngle);
            Vector2 branchPos = tipPos.ExtendBy(60f * Mathf.Sin(5f * i / repeatTimes), aimAngle + (i % 2 == 0 ? 90f : -90f));
            float halfFanSpread = (bullet - 1) * spread / 2f;
            float waveAimAngle = new Vector3(branchPos.x, branchPos.y).AngleTo(aimPos);
            for (int j = 0; j < bullet; j++)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(branchPos, 1.7f, aimAngle - halfFanSpread + spread * j, i % 2 == 0 ? enemyBulletType1 : enemyBulletType2, 4);
            }
            yield return WaitForFrames.WaitWrapper(delayBetweenWaves);
        }
    }
}

