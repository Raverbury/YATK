using System.Collections.Generic;
using Assets.Scripts.Util;
using MEC;
using STG;
using UnityEngine;

public class StageExChapter3 : AbstractSingle
{
    public int GetHP()
    {
        return 0;
    }

    public override string GetName()
    {
        return "EX C3";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 24;
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

    protected override IEnumerator<float> _Loop()
    {
        yield return WaitForFrames.WaitWrapper(60);
        for (int i = 0; i < 3; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(192f - 150f + 150f * i));
        }

        yield return WaitForFrames.WaitWrapper(75);
        for (int i = 0; i < 2; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(192f - 75f + 150f * i));
        }
        yield return WaitForFrames.WaitWrapper(360);

        int vertSide = 1;
        for (int i = 0; i < 4; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnCurvingFairy(1, vertSide));
            CoroutineUtil.StartSingleLoopCRT(_SpawnCurvingFairy(-1, vertSide));
            yield return WaitForFrames.WaitWrapper(220);
            vertSide *= -1;
        }

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _SpawnFairy(float xPos)
    {
        Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_GREEN), 180, new() { new ItemStack(ItemType.POWER_ITEM, 6) }, xPos, 100);
        CoroutineUtil.StartSingleLoopCRT(_FairyMove(fairyEnemy).CancelWith(fairyEnemy.gameObject));
        yield break;
    }

    private IEnumerator<float> _FairyMove(Enemy fairy)
    {
        // move down
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, -160f), 60).CancelWith(fairy.gameObject)));
        // start shooting
        Timing.RunCoroutine(_FairyShoot1(fairy).CancelWith(fairy.gameObject));
        yield return WaitForFrames.WaitWrapper(210);
        Timing.RunCoroutine(_FairyShoot1(fairy).CancelWith(fairy.gameObject));
        // move up and despawn
        yield return WaitForFrames.WaitWrapper(10);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, 240), 150).CancelWith(fairy.gameObject)));


        // TODO: rebalance all mokou singles' hp, check timing

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot1(Enemy fairy)
    {
        // continuous attack
        float angleToPlayer = fairy.transform.position.AngleTo(Player.instance.transform.position);
        for (int i = 0; i < 7; i++)
        {
            if (fairy && !fairy.IsDead() && Player.instance)
            {
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f + 0.4f * i * (1 + 0.17f * i), angleToPlayer, EnemyBulletType.ARROW_ORANGE, 5);
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(-10f, 15f).RotateBy(angleToPlayer), 3f + 0.4f * i * (1 + 0.17f * i), angleToPlayer, EnemyBulletType.ARROW_ORANGE, 7);
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(-10f, -15f).RotateBy(angleToPlayer), 3f + 0.4f * i * (1 + 0.17f * i), angleToPlayer, EnemyBulletType.ARROW_ORANGE, 7);
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(-20f, 30f).RotateBy(angleToPlayer), 3f + 0.4f * i * (1 + 0.17f * i), angleToPlayer, EnemyBulletType.ARROW_ORANGE, 9);
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(-20f, -30f).RotateBy(angleToPlayer), 3f + 0.4f * i * (1 + 0.17f * i), angleToPlayer, EnemyBulletType.ARROW_ORANGE, 9);
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(9);
        }
    }

    const int CURVING_FAIRY_COUNT = 15;
    const float SIDE_X_OFFSET = 150f;
    const float SIDE_Y_OFFSET = 260f;
    private IEnumerator<float> _SpawnCurvingFairy(int side, int vertSide)
    {
        for (int i = 0; i < CURVING_FAIRY_COUNT; i++)
        {
            Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_RED), 2, new List<ItemStack>(), 192 + side * SIDE_X_OFFSET, -224 + vertSide * SIDE_Y_OFFSET);
            Timing.RunCoroutine(_CurvingFairyMove(fairyEnemy, side, vertSide).CancelWith(fairyEnemy.gameObject));
            yield return WaitForFrames.WaitWrapper(10);
        }
    }

    private IEnumerator<float> _CurvingFairyMove(Enemy fairy, int side, int vertSide)
    {
        const float ARC = 180f;
        const int CIRCLE_DURATION = 140;
        const float ANGULAR_VEL = ARC / CIRCLE_DURATION;
        const float SPEED = ANGULAR_VEL * Mathf.Deg2Rad * SIDE_X_OFFSET;

        float targetY = -224 - vertSide * 30;
        float yDistance = Mathf.Abs(fairy.transform.position.y - targetY);

        // move up from bottom of screen/top of screen depending on vertSide
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, targetY), (int)(yDistance / SPEED)).CancelWith(fairy.gameObject)));
        // start shooting
        Timing.RunCoroutine(_CurvingFairyShoot(fairy).CancelWith(fairy.gameObject));
        // do a 180 deg arc
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyCircular(90f - 90f * side, ANGULAR_VEL * side * -vertSide, SPEED, CIRCLE_DURATION).CancelWith(fairy.gameObject)));
        // continue moving straight out of screen
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, fairy.transform.position.y + 300f * vertSide), (int)(300 / SPEED)).CancelWith(fairy.gameObject)));

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _CurvingFairyShoot(Enemy fairy)
    {
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
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f, angleToPlayer, EnemyBulletType.RICE_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f, angleToPlayer + 80f, EnemyBulletType.RICE_DARK_RED, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 3f, angleToPlayer - 80f, EnemyBulletType.RICE_DARK_GREEN, 5);
                    SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
                }
            }
            yield return WaitForFrames.WaitWrapper(45);
        }
    }
}

