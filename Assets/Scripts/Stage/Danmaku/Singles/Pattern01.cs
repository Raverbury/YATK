using System.Collections.Generic;
using MEC;
using STG;
using Unity.Entities;
using UnityEngine;

public class Pattern01 : AbstractSingle
{
    public int GetHP()
    {
        return 2300;
    }

    public override string GetName()
    {
        return "Origin Sign [Asymmetric Pursuit]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 32;
    }

    protected override bool IsBossAttack()
    {
        return true;
    }

    protected override bool IsSpellCard()
    {
        return true;
    }

    protected override bool IsTimeout()
    {
        return false;
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
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.POWER_ITEM, 7),
            new ItemStack(ItemType.BOMB_ITEM, 1),
            new ItemStack(ItemType.BIG_POWER_ITEM, 1),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        const int SEGMENTS = 8;
        const int BRANCHES = SEGMENTS * 11;
        int rotation = 90;
        bool oddWave = true;
        while (true)
        {
            float r = Random.Range(0, 10);
            for (int i = 0; i < BRANCHES; i++)
            {
                // GameObject bullet = pool.SpawnBulletA1(192, -60, 2 + 3 * ((-1 * Mathf.Abs(i - BRANCHES / 2)) + BRANCHES / 2) / (BRANCHES / 2), 360f / (BRANCHES / 2) * i + r, 0.5f);
                int j = (i % SEGMENTS) switch
                {
                    0 or 7 => 1,
                    1 or 6 => 2,
                    2 or 5 => 3,
                    _ => 2,
                };
                float speed = 2 + 0.5f * j;
                float angle = 360f / BRANCHES * i + rotation;
                EnemyBulletType color = oddWave ? EnemyBulletType.ARROW_DARK_BLUE : EnemyBulletType.ARROW_DARK_GREEN;
                Entity bullet = ECSEntitySpawner.SpawnEnemyBulletE1(enemy.transform.position, speed, angle, color, 30);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(bullet), bullet);
            }
            rotation += 7;
            oddWave = !oddWave;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(45)));
        }
    }

    IEnumerator<float> _Manipulate(Entity bulletEntity)
    {
        int i = 0;
        while (!ECSEntitySpawner.EntityIsDisabled(bulletEntity) && i < 100)
        {
            if (i == 99)
            {
                ECSEntitySpawner.SetBulletSpeed(bulletEntity, 1.2f);
            }
            i += 1;
            yield return Timing.WaitForOneFrame;
        }
    }
}
