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
        return "Dream Sign [Dream of Vivid Nightmare]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 80;
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
        if (enemy) {
            enemy.SetEmptyHpCircle();
        }
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.BIG_POWER_ITEM, 2),
        });
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
        targetPos.y = Mathf.Clamp(targetPos.y, Constant.GAME_BORDER_BOTTOM + 100, Constant.GAME_BORDER_TOP - 100);
        // spawn init ring
        const float OUTER_RADIUS = 120f;
        const int OUTER_COUNT = 80;
        const float OUTER_SPREAD = 360f / OUTER_COUNT;
        SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_KIRA00, 0.2f);
        for (int i = 0; i < OUTER_COUNT * 2; i++)
        {
            float facing = 90f + 90f * dir - OUTER_SPREAD * i * dir;
            float bulletX = targetPos.x + Mathf.Cos(facing * Mathf.Deg2Rad) * OUTER_RADIUS;
            float bulletY = targetPos.y + Mathf.Sin(facing * Mathf.Deg2Rad) * OUTER_RADIUS;
            Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0.03f, facing - 180, EnemyBulletType.BALL2_DARK_YELLOW, 5);
            CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity, 2f, 390 + i), entity);
        }
        yield return WaitForFrames.WaitWrapper(10);

        // spawn circle closing in
        const float INNER_RADIUS = 100f;
        const int INNER_COUNT = 80;
        const float INNER_SPREAD = 360f / OUTER_COUNT;
        SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_KIRA00, 0.2f);
        for (int i = 0; i < INNER_COUNT * 2; i++)
        {
            float radius = INNER_RADIUS + 0.2f * i;
            float facing = 90f - 90f * dir - INNER_SPREAD * i * dir;
            float bulletX = targetPos.x + Mathf.Cos(facing * Mathf.Deg2Rad) * radius;
            float bulletY = targetPos.y + Mathf.Sin(facing * Mathf.Deg2Rad) * radius;
            for (int j = 0; j < 3; j++)
            {
                Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletX, bulletY, 0.01f, facing - 180f + 120f * j, EnemyBulletType.RICE_DARK_GREEN, 5);
                CoroutineUtil.RunEntityBoundCoroutine(_Manipulate(entity, 1.5f, 10 + (2 * INNER_COUNT - i) * 2), entity);
            }
            yield return Timing.WaitForOneFrame;
        }
    }


    IEnumerator<float> _Manipulate(Entity entity, float speed, int wait)
    {
        yield return WaitForFrames.WaitWrapper(wait);
        ECSEntitySpawner.SetBulletSpeed(entity, speed);
    }
}
