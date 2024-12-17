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
        return "Dream Sign [Fickling Embrace]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 20;
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
        if (!hasSpawnedFairies)
        {
            return false;
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy fairy = enemies[i];
            if (fairy && !fairy.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    protected override void CleanUp()
    {
        StageManager.ClearEnemyBullet(true, true);
    }

    private List<Enemy> enemies = new();
    private bool hasSpawnedFairies = false;

    const int FAIRY_COUNT = 10;
    const int GAP = 20;
    const float HALF_WIDTH = GAP * (FAIRY_COUNT - 1) / 2f;

    protected override IEnumerator<float> _Loop()
    {
        enemies = new();
        hasSpawnedFairies = false;
        yield return WaitForFrames.WaitWrapper(420);
        for (int i = 0; i < 10; i++)
        {
            Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_RED), 5, new(), 192 - 150, -500f);
            enemies.Add(fairyEnemy);
            CoroutineUtil.StartSingleLoopCRT(_LoopFairy(fairyEnemy, 1).CancelWith(fairyEnemy.gameObject));
            yield return WaitForFrames.WaitWrapper(10);
        }
        hasSpawnedFairies = true;
        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _LoopFairy(Enemy fairy, int side)
    {
        yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 - 150 * side, -180), 90).CancelWith(fairy.gameObject)));
        CoroutineUtil.StartSingleLoopCRT(_FairyShoot(fairy).CancelWith(fairy.gameObject));
        yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(fairy._MoveEnemyCircular(180f, -270f / 210, 270f / 210 * Mathf.Deg2Rad * 150f, 210).CancelWith(fairy.gameObject)));
        yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 + 300, -300), 250).CancelWith(fairy.gameObject)));

        // yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(fairy.transform.position + new Vector3(0f, 120f), 90).CancelWith(fairy.gameObject)));
        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot(Enemy fairy)
    {
        for (int i = 0; i < 5; i++)
        {
            if (Player.instance != null)
            {
                if (fairy && !fairy.IsDead())
                {
                    float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                        Player.instance.transform.position.y - fairy.transform.position.y,
                        Player.instance.transform.position.x - fairy.transform.position.x
                    );
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 5f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.9f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.8f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.7f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.6f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.5f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.4f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.3f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.2f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4.1f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 4f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                    SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.5f);
                }
            }
            yield return WaitForFrames.WaitWrapper(55);
        }
    }
}

