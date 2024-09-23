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
        return "Nonspell 4";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 50;
    }

    public override bool IsTimeout()
    {
        return false;
    }

    const int FLOWERS = 40;
    const int FLOWER_PETALS = 5;
    const float FLOWER_RING_RADIUS = 80f;

    protected override IEnumerator<float> _Loop(Enemy enemy)
    {
        int wait = 60;
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));

        enemy.SetAnimState(Enemy.AnimState.Attack);
        while (true)
        {
            Vector2 pos = (Player.instance == null) ? new Vector2(STG.Constant.GAME_CENTER_X, STG.Constant.GAME_CENTER_Y) : Player.instance.transform.position;
            for (int i = 0; i < 2; i++)
            {
                CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(Constant.GAME_BORDER_LEFT, pos.y, Mathf.Abs(Constant.GAME_BORDER_LEFT - pos.x) * 0.005f + 2 * i, 0f, EnemyBulletType.ARROW_RED, 20)));
                CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(pos.x, Constant.GAME_BORDER_TOP, Mathf.Abs(Constant.GAME_BORDER_TOP - pos.y) * 0.008f + 2 * i, 270f, EnemyBulletType.ARROW_GREEN, 20)));
                CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(Constant.GAME_BORDER_RIGHT, pos.y, Mathf.Abs(Constant.GAME_BORDER_RIGHT - pos.x) * 0.005f + 2 * i, 180f, EnemyBulletType.ARROW_BLUE, 20)));
                for (int j = 0; j < 10; j++)
                {
                    float posX = j switch
                    {
                        0 => 12,
                        1 => 32,
                        2 => 52,
                        3 => 72,
                        4 => 92,
                        5 => 292,
                        6 => 312,
                        7 => 332,
                        8 => 352,
                        _ => 372,
                    };
                    CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 3f, 90f, EnemyBulletType.ARROW_LIGHT_YELLOW, 20)));
                    CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 4f, 90f, EnemyBulletType.ARROW_YELLOW, 20)));
                    CoroutineUtil.StartSingleLoopCRT(_Manipulate(EnemyBulletPool.SpawnBulletA1(posX, Constant.GAME_BORDER_BOTTOM - 20f, 5f, 90f, EnemyBulletType.ARROW_DARK_YELLOW, 20)));
                }
            }

            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
            wait = Mathf.Max(30, wait - 1);
        }
    }

    private IEnumerator<float> _Manipulate(GameObject gameObject)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(5 * 60)));
        gameObject.SetActive(false);
    }
}
