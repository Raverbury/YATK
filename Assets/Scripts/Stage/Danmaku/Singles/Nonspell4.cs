using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class Nonspell4 : AbstractSingle
{
    public int GetHP()
    {
        return 4800;
    }

    public override string GetName()
    {
        return "Collapse Matrix";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 50;
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

    protected override void CleanUp()
    {
        if (enemy) {
            enemy.SetEmptyHpCircle();
        }
    }

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        return enemy && enemy.IsDead();
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.POWER_ITEM, 13),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        enemy.SetAnimState(Enemy.AnimState.Attack);
        int wait = 60;
        while (true)
        {
            Vector2 pos = (Player.instance == null) ? new Vector2(STG.Constant.GAME_CENTER_X, STG.Constant.GAME_CENTER_Y) : Player.instance.transform.position;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float rotOffset = -60f + 60f * j;
                    ECSEntitySpawner.SpawnEnemyBulletE1(Constant.GAME_BORDER_LEFT, pos.y, Mathf.Abs(Constant.GAME_BORDER_LEFT - pos.x) * 0.005f + 2 * i, 0f + rotOffset, EnemyBulletType.ARROW_RED, 20);
                    ECSEntitySpawner.SpawnEnemyBulletE1(pos.x, Constant.GAME_BORDER_TOP, Mathf.Abs(Constant.GAME_BORDER_TOP - pos.y) * 0.008f + 1.5f * i, 270f + rotOffset, EnemyBulletType.ARROW_GREEN, 20);
                    ECSEntitySpawner.SpawnEnemyBulletE1(Constant.GAME_BORDER_RIGHT, pos.y, Mathf.Abs(Constant.GAME_BORDER_RIGHT - pos.x) * 0.005f + 2 * i, 180f + rotOffset, EnemyBulletType.ARROW_BLUE, 20);
                }
                for (int j = 0; j < 8; j++)
                {
                    float posX = j switch
                    {
                        0 => 12,
                        1 => 32,
                        2 => 52,
                        3 => 72,
                        4 => 312,
                        5 => 332,
                        6 => 352,
                        _ => 372,
                    };
                    ECSEntitySpawner.SpawnEnemyBulletE1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 3f, 90f, EnemyBulletType.ARROW_LIGHT_YELLOW, 20);
                    ECSEntitySpawner.SpawnEnemyBulletE1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 4f, 90f, EnemyBulletType.ARROW_YELLOW, 20);
                    ECSEntitySpawner.SpawnEnemyBulletE1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 5f, 90f, EnemyBulletType.ARROW_DARK_YELLOW, 20);
                }
            }

            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
            wait = Mathf.Max(30, wait - 1);
        }
    }
}
