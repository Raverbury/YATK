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
        return "Nonspell 3";
    }

    public override int GetScore()
    {
        return 0;
    }

    public override int GetTimer()
    {
        return 40;
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
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60)));
        AbstractSingle.PatternStart?.Invoke();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));

        while (true)
        {
            Vector2 pos = (Player.instance == null) ? new Vector2(STG.Constant.GAME_CENTER_X, STG.Constant.GAME_CENTER_Y) : Player.instance.transform.position;
            CoroutineUtil.StartSingleLoopCRT(_Manipulate(StageManager.SpawnBulletA1(STG.Constant.GAME_BORDER_LEFT, pos.y, Mathf.Abs(STG.Constant.GAME_BORDER_LEFT - pos.x) * 0.011f, 270f, EnemyBulletType.ARROW_RED, 20)));
            CoroutineUtil.StartSingleLoopCRT(_Manipulate(StageManager.SpawnBulletA1(pos.x, STG.Constant.GAME_BORDER_TOP, Mathf.Abs(STG.Constant.GAME_BORDER_TOP - pos.y) * 0.015f, 180f, EnemyBulletType.ARROW_YELLOW, 20)));
            CoroutineUtil.StartSingleLoopCRT(_Manipulate(StageManager.SpawnBulletA1(STG.Constant.GAME_BORDER_RIGHT, pos.y, Mathf.Abs(STG.Constant.GAME_BORDER_RIGHT - pos.x) * 0.011f, 90f, EnemyBulletType.ARROW_BLUE, 20)));
            CoroutineUtil.StartSingleLoopCRT(_Manipulate(StageManager.SpawnBulletA1(pos.x, STG.Constant.GAME_BORDER_BOTTOM, Mathf.Abs(STG.Constant.GAME_BORDER_BOTTOM - pos.y) * 0.015f, 0f, EnemyBulletType.ARROW_GREEN, 20)));

            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(30)));
        }
    }

    private IEnumerator<float> _Manipulate(GameObject gameObject)
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(5 * 60)));
        gameObject.SetActive(false);
    }
}
