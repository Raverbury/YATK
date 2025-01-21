using System.Collections.Generic;
using MEC;
using STG;
using Unity.VisualScripting;
using UnityEngine;

public class StageExChapter4 : AbstractSingle
{
    public int GetHP()
    {
        return 0;
    }

    public override string GetName()
    {
        return "EX C4";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 12;
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
        CoroutineUtil.StartSingleLoopCRT(_SpawnFairy());

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _SpawnFairy()
    {
        Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_GREEN), 1900, new() { new ItemStack(ItemType.LIFE_ITEM, 1) }, 192, 100);
        CoroutineUtil.StartSingleLoopCRT(_FairyMove(fairyEnemy).CancelWith(fairyEnemy.gameObject));
        yield break;
    }

    private IEnumerator<float> _FairyMove(Enemy fairy)
    {
        // move down
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192, -130f), 60).CancelWith(fairy.gameObject)));
        // start shooting
        Timing.RunCoroutine(_FairyShoot1(fairy).CancelWith(fairy.gameObject));
        // move up and despawn
        yield return WaitForFrames.WaitWrapper(540);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, 240), 90).CancelWith(fairy.gameObject)));


        // TODO: rebalance all mokou singles' hp, check timing

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot1(Enemy fairy)
    {
        // continuous attack
        const int BRANCHES = 3;
        const float SPREAD = 7f;
        const float HALF_FAN_SPREAD = SPREAD * (BRANCHES - 1) * 0.5f;
        int rot = 0;
        float ringRadius = 120f;
        for (int i = 0; i < 150; i++)
        {
            if (fairy && !fairy.IsDead())
            {
                float currentRadius = 30f + ringRadius * Mathf.Cos(Mathf.PI * i / 45f);
                float redAngle = (270f + rot) % 360f;
                float blueAngle = (270f - rot) % 360f;
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * redAngle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * redAngle)), 1.5f, redAngle + 180f, EnemyBulletType.ICE_BLUE, 5);
                ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * blueAngle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * blueAngle)), 1.5f, blueAngle + 180f, EnemyBulletType.ICE_RED, 5);
                for (int j = 0; j < BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * redAngle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * redAngle)), 1.6f, redAngle - HALF_FAN_SPREAD + SPREAD * j, EnemyBulletType.ICE_RED, 5);
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * blueAngle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * blueAngle)), 1.6f, blueAngle - HALF_FAN_SPREAD + SPREAD * j, EnemyBulletType.ICE_BLUE, 5);
                }
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(3);
            rot += 17;
        }
    }
}

