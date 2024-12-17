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

    const int FAIRY_COUNT = 10;
    const int GAP = 20;
    const float HALF_WIDTH = GAP * (FAIRY_COUNT - 1) / 2f;

    protected override IEnumerator<float> _Loop()
    {
        enemies = new();
        for (int i = 0; i < 10; i++)
        {
            Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_RED), 5, new(), 192 - HALF_WIDTH + GAP * i);
            enemies.Add(fairyEnemy);
            CoroutineUtil.StartSingleLoopCRT(_LoopFairy(fairyEnemy, i).CancelWith(fairyEnemy.gameObject));
        }
        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _LoopFairy(Enemy fairy, int index)
    {
        yield return Timing.WaitUntilDone(CoroutineUtil.StartSingleLoopCRT(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 - HALF_WIDTH + GAP * index, -150), 70).CancelWith(fairy.gameObject)));
        for (int i = 0; i < 13; i++)
        {
            if (Player.instance != null)
            {
                if (fairy && !fairy.IsDead())
                {
                    float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(
                        Player.instance.transform.position.y - fairy.transform.position.y,
                        Player.instance.transform.position.x - fairy.transform.position.x
                    );
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2f, angleToPlayer, EnemyBulletType.BALL2_BLUE, 5);
                }
            }
            yield return WaitForFrames.WaitWrapper(60);
        }
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192 + (-HALF_WIDTH + GAP * index) * 4f, 220), 70).CancelWith(fairy.gameObject)));
        StageManager.DestroyEnemy(fairy);
    }
}

