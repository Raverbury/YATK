using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;

public class Nonspell13 : AbstractSingle
{
    public int GetHP()
    {
        return 5200;
    }

    public override string GetName()
    {
        return "Nonspell 7";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 60;
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
            SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.2f);
            for (int i = 0; i < BRANCHES; i++)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, speed, r + SPREAD * i, EnemyBulletType.AMULET_RED, 5);
            }
            yield return WaitForFrames.WaitWrapper(45);
            r = Random.Range(0f, 360f);
            spawnPos = rightSpawn + Random.insideUnitCircle * 60f;
            speed = Random.Range(1.8f, 3f);
            SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.2f);
            for (int i = 0; i < BRANCHES; i++)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(spawnPos, speed, r + SPREAD * i, EnemyBulletType.AMULET_PURPLE, 5);
            }
            yield return WaitForFrames.WaitWrapper(45);
        }
    }
}

