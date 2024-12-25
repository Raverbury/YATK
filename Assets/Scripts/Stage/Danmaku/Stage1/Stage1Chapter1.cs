using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class Stage1Chapter1 : AbstractSingle
{
    public int GetHP()
    {
        return 10000;
    }

    public override string GetName()
    {
        return "EX C1";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 19;
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

    private List<Enemy> enemies = new();
    private bool hasSpawnedFairies = false;

    const int FAIRY_COUNT = 10;
    const float SIDE_X_OFFSET = 150f;

    protected override IEnumerator<float> _Loop()
    {
        enemies = new();
        hasSpawnedFairies = false;
        yield return WaitForFrames.WaitWrapper(420);
        int side = 1;
        for (int i = 0; i < 3; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(side));
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(-side));
            yield return WaitForFrames.WaitWrapper(185);
            side *= -1;
        }

        hasSpawnedFairies = true;
        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _SpawnFairy(int side)
    {
        for (int i = 0; i < FAIRY_COUNT; i++)
        {
            Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_RED), 5, new(), 192 + side * SIDE_X_OFFSET, -500f);
            enemies.Add(fairyEnemy);
            Timing.RunCoroutine(_FairyMove(fairyEnemy, side).CancelWith(fairyEnemy.gameObject));
            yield return WaitForFrames.WaitWrapper(10);
        }
    }

    private IEnumerator<float> _FairyMove(Enemy fairy, int side)
    {
        const float ARC = 270f;
        const int CIRCLE_DURATION = 210;
        const float ANGULAR_VEL = ARC / CIRCLE_DURATION;
        const float SPEED = ANGULAR_VEL * Mathf.Deg2Rad * SIDE_X_OFFSET;

        // move up from bottom of screen
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 + SIDE_X_OFFSET * side, -180f), (int)(320 / SPEED)).CancelWith(fairy.gameObject)));
        // start shooting
        Timing.RunCoroutine(_FairyShoot(fairy).CancelWith(fairy.gameObject));
        // do a 270deg arc
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyCircular(90f - 90f * side, ANGULAR_VEL * side, SPEED, 210).CancelWith(fairy.gameObject)));
        // continue moving straight out of screen
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x + 300f * side, fairy.transform.position.y), (int)(300 / SPEED)).CancelWith(fairy.gameObject)));

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot(Enemy fairy)
    {
        for (int i = 0; i < 6; i++)
        {
            if (Player.instance != null)
            {
                if (fairy && !fairy.IsDead())
                {
                    float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                        Player.instance.transform.position.y - fairy.transform.position.y,
                        Player.instance.transform.position.x - fairy.transform.position.x
                    );
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 6f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.9f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.8f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.7f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.6f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.5f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.4f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.3f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.2f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5.1f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5f, angleToPlayer, EnemyBulletType.BALL2_DARK_BLUE, 5);
                    SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
                }
            }
            yield return WaitForFrames.WaitWrapper(25);
        }
    }
}

