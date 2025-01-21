using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell10 : AbstractSingle
{
    public int GetHP()
    {
        return 4200;
    }

    public override string GetName()
    {
        return "Nonspell 4";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 45;
    }

    protected override bool IsTimeout()
    {
        return false;
    }

    protected override bool IsSpellCard()
    {
        return false;
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
        if (enemy) {
            enemy.SetEmptyHpCircle();
        }
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new());
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        const int BURSTS = 3;
        int branches = 60;
        // int i = 0;
        float branchRotation = 360f / branches;
        float halfBranchRotation = branchRotation / 2f;
        CoroutineUtil.StartSingleLoopCRT(_MoveEnemy(enemy));
        while (true)
        {
            SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.2f);
            float angleToPlayer = 270f;
            if (Player.instance != null)
            {
                angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                    enemy.transform.position.y - Player.instance.transform.position.y,
                    enemy.transform.position.x - Player.instance.transform.position.x
                );
            }
            for (int i = 0; i < BURSTS; i++)
            {
                int r = Random.Range(0, 2);
                for (int j = 0; j < branches; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, 4f + 1.5f * i, halfBranchRotation * r + angleToPlayer + branchRotation * j, EnemyBulletType.ICE_PURPLE, 10);
                }
            }
            branches = Mathf.Min(branches + 1, 70);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(90)));
        }
    }

    private IEnumerator<float> _MoveEnemy(Enemy enemy)
    {
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(90)));
            float targetX = ((Player.instance == null) ? 192f : Player.instance.gameObject.transform.position.x) + Random.Range(-20f, 20f);
            targetX = Mathf.Clamp(targetX, Constant.GAME_BORDER_LEFT + 60, Constant.GAME_BORDER_RIGHT - 60);
            yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(enemy._MoveEnemyToOver(new Vector2(targetX, Random.Range(-60, -60)), 60)));
            enemy.SetAnimState(Enemy.AnimState.Attack);
        }
    }
}

