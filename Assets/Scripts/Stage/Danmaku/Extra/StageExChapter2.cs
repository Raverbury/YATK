using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class StageExChapter2 : AbstractSingle
{
    public int GetHP()
    {
        return 0;
    }

    public override string GetName()
    {
        return "EX C2";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 36;
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
        return false;
    }

    protected override LoopableBGM SingleBGM()
    {
        return RuntimeGameData.Registry.BGM_08_EXTRA;
    }

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        return false;
    }

    protected override void CleanUp()
    {
        // StageManager.ClearEnemyBullet(true, true);
    }

    const float SIDE_X_OFFSET = 250f;

    protected override IEnumerator<float> _Loop()
    {
        yield return WaitForFrames.WaitWrapper(60);
        int side = 1;
        for (int i = 0; i < 4; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(side));
            yield return WaitForFrames.WaitWrapper(300);
            side *= -1;
        }

        yield return WaitForFrames.WaitWrapper(30);

        for (int i = 0; i < 2; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnSidewayFairy(side));
            yield return WaitForFrames.WaitWrapper(290);
            side *= -1;
        }

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _SpawnFairy(int side)
    {
        Enemy fairyEnemy1 = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_GREEN), 90, new() { new ItemStack(ItemType.POWER_ITEM, 6) }, 192 + side * SIDE_X_OFFSET, -100);
        CoroutineUtil.StartSingleLoopCRT(_FairyMove(fairyEnemy1, side).CancelWith(fairyEnemy1.gameObject));
        yield return WaitForFrames.WaitWrapper(45);
        Enemy fairyEnemy2 = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_YELLOW), 90, new() { new ItemStack(ItemType.POWER_ITEM, 6) }, 192 - side * SIDE_X_OFFSET, -100);
        CoroutineUtil.StartSingleLoopCRT(_FairyMove(fairyEnemy2, -side).CancelWith(fairyEnemy2.gameObject));
    }

    private IEnumerator<float> _FairyMove(Enemy fairy, int side)
    {
        const float IN_PLACE_X_OFFSET = 80f;

        // move diagonal from side into screen
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 - IN_PLACE_X_OFFSET * side, -150f), 50).CancelWith(fairy.gameObject)));
        // start shooting
        Timing.RunCoroutine(_FairyShoot(fairy, side).CancelWith(fairy.gameObject));
        // move up and despawn
        yield return WaitForFrames.WaitWrapper(240);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, 240), 80).CancelWith(fairy.gameObject)));

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot(Enemy fairy, int side)
    {
        float angleToPlayer = 270f;
        if (Player.instance != null)
        {
            angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                Player.instance.transform.position.y - fairy.transform.position.y,
                Player.instance.transform.position.x - fairy.transform.position.x
            );
        }
        // initial concentrated blast
        // ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3.8f, angleToPlayer, EnemyBulletType.BUBBLE_DARK_GREEN, 5);
        for (int i = 0; i < 100; i++)
        {
            ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3.5f + Random.Range(-0.6f, 0.6f), angleToPlayer + Random.Range(-3f, 3f), EnemyBulletType.BALL2_LIME, 5);
            SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.25f);
        }
        // continuous attack
        const int BRANCHES = 5;
        const float GAP = 360f / BRANCHES;
        float angle = angleToPlayer;
        for (int i = 0; i < 30; i++)
        {
            if (Player.instance != null)
            {
                if (fairy && !fairy.IsDead())
                {
                    for (int j = 0; j < BRANCHES; j++)
                    {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f, angle + 45f + GAP * j, EnemyBulletType.AMULET_DARK_BLUE, 5);
                    }
                    for (int j = 0; j < BRANCHES; j++)
                    {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f, 180 - angle - 45f + GAP * j, EnemyBulletType.AMULET_DARK_RED, 5);
                    }
                    SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
                }
            }
            yield return WaitForFrames.WaitWrapper(10);
            angle += 8.3f;
        }
    }

    const int SIDEWAY_X_OFFSET = 200;
    private IEnumerator<float> _SpawnSidewayFairy(int side)
    {
        const int FAIRY_COUNT = 10;
        for (int i = 0; i < FAIRY_COUNT; i++)
        {
            Enemy fairyEnemy1 = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_GREEN), 5, new(), 192 + side * SIDEWAY_X_OFFSET, -50);
            CoroutineUtil.StartSingleLoopCRT(_SidewayFairyMove(fairyEnemy1, side).CancelWith(fairyEnemy1.gameObject));
            Enemy fairyEnemy2 = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_YELLOW), 5, new(), 192 - side * SIDEWAY_X_OFFSET, -398);
            CoroutineUtil.StartSingleLoopCRT(_SidewayFairyMove(fairyEnemy2, -side).CancelWith(fairyEnemy2.gameObject));
            yield return WaitForFrames.WaitWrapper(25);
        }
    }

    private IEnumerator<float> _SidewayFairyMove(Enemy fairy, int side)
    {
        // start shooting
        Timing.RunCoroutine(_SidewayFairyShoot(fairy).CancelWith(fairy.gameObject));
        // move sideway to other side of screen and despawn
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 - SIDE_X_OFFSET * side, fairy.transform.position.y), 250).CancelWith(fairy.gameObject)));

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _SidewayFairyShoot(Enemy fairy)
    {
        yield return WaitForFrames.WaitWrapper(5);
        for (int i = 0; i < 2; i++)
        {
            if (Player.instance != null)
            {
                if (fairy && !fairy.IsDead())
                {
                    float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                        Player.instance.transform.position.y - fairy.transform.position.y,
                        Player.instance.transform.position.x - fairy.transform.position.x
                    );
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2.6f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2.6f, angleToPlayer + 1f, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2.6f, angleToPlayer - 1f, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
                }
            }
            yield return WaitForFrames.WaitWrapper(90);
        }
    }
}

