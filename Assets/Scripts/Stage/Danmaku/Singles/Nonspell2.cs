using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class Nonspell2 : AbstractSingle
{
    public int GetHP()
    {
        return 4000;
    }

    public override string GetName()
    {
        return "Nonspell 1";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 35;
    }

    protected override bool IsBossAttack()
    {
        return true;
    }

    protected override bool IsSpellCard()
    {
        return false;
    }

    protected override bool IsTimeout()
    {
        return false;
    }

    protected override LoopableBGM SingleBGM()
    {
        return RuntimeGameData.Registry.BGM_08_MOKOU;
    }

    protected override void CleanUp()
    {
        if (enemy) {
            enemy.SetEmptyHpCircle();
        }
    }

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        bool res = enemy && enemy.IsDead();
        if (res) {
            enemy.DropRewards();
        }
        return res;
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.POWER_ITEM, 4),
            new ItemStack(ItemType.POINT_ITEM, 5),
            new ItemStack(ItemType.BIG_POWER_ITEM, 1),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        const int BRANCHES = 10;
        const int BURSTS = 5;
        const float SPEED = 3;
        int dir = 17;
        int rotation = 90;
        float angle = 360f / BRANCHES;
        while (true)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(25)));
            SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.2f);
            for (int i = 0; i < BURSTS; i++)
            {
                EnemyBulletType enemyBulletType = i switch
                {
                    0 => EnemyBulletType.AMULET_RED,
                    1 => EnemyBulletType.AMULET_SKY,
                    2 => EnemyBulletType.AMULET_GREEN,
                    3 => EnemyBulletType.AMULET_ORANGE,
                    _ => EnemyBulletType.AMULET_YELLOW,
                };
                for (int j = 0; j < BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(enemy.gameObject.transform.position, SPEED * (1 - i * 0.1f), angle * j + rotation + i * dir, enemyBulletType, 0);
                }
            }
            if (Random.Range(0f, 1f) < 0.5f)
            {
                dir *= -1;
            }
            rotation = Random.Range(0, 361);
        }
    }
}
