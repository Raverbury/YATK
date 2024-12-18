using System.Collections.Generic;
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
        return 50;
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
        int side = 1;
        for (int i = 0; i < 4; i++)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnFairy(i));
            yield return WaitForFrames.WaitWrapper(500);
            side *= -1;
        }

        yield return WaitForFrames.WaitWrapper(30);

        while (true)
        {
            yield return 1;
        }
    }

    private IEnumerator<float> _SpawnFairy(int index)
    {
        Enemy fairyEnemy = SpawnFairyEnemyUtil(ShotSheet.GetFairyEnemyData(FairyType.FAIRY_GREEN), 900, new() { new ItemStack(ItemType.POWER_ITEM, 6) }, 192, 100);
        CoroutineUtil.StartSingleLoopCRT(_FairyMove(fairyEnemy, index).CancelWith(fairyEnemy.gameObject));
        yield break;
    }

    private IEnumerator<float> _FairyMove(Enemy fairy, int index)
    {
        // move down
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(192, -130f), 60).CancelWith(fairy.gameObject)));
        // start shooting
        switch (index) {
            case 0:
                Timing.RunCoroutine(_FairyShoot1(fairy).CancelWith(fairy.gameObject));
                break;
            case 1:
                Timing.RunCoroutine(_FairyShoot2(fairy).CancelWith(fairy.gameObject));
                break;
            case 2:
                Timing.RunCoroutine(_FairyShoot3(fairy).CancelWith(fairy.gameObject));
                break;
            default:
                Timing.RunCoroutine(_FairyShoot4(fairy).CancelWith(fairy.gameObject));
                break;
        }
        // move up and despawn
        yield return WaitForFrames.WaitWrapper(600);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(fairy._MoveEnemyToOverFairyStyle(new Vector2(fairy.transform.position.x, 240), 90).CancelWith(fairy.gameObject)));


        // TODO: rebalance all mokou singles' hp, check timing

        StageManager.DestroyEnemy(fairy);
    }

    private IEnumerator<float> _FairyShoot1(Enemy fairy)
    {
        // continuous attack
        const int RED_BRANCHES = 6;
        const float GAP = 360f / RED_BRANCHES;
        const int BLUE_BRANCHES = 3;
        const float BLUE_GAP = 360f / BLUE_BRANCHES;
        float angle = 180;
        float ringRadius = 130f;
        for (int i = 0; i < 80; i++)
        {
            if (fairy && !fairy.IsDead())
            {
                float currentRadius = ringRadius * Mathf.Cos(Mathf.PI * i / 60f);
                if (i % 3 == 0)
                {
                    for (int j = 0; j < BLUE_BRANCHES; j++) {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2f, -angle - BLUE_GAP * j, EnemyBulletType.BALL2_BLUE, 5);
                    }
                }
                for (int j = 0; j < RED_BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * (angle + GAP * j)), currentRadius * Mathf.Sin(Mathf.Deg2Rad * (angle + GAP * j))), 1.6f, angle + GAP * j, EnemyBulletType.BALL2_DARK_RED, 5);
                }
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(5);
            angle += -21f;
        }
    }

    private IEnumerator<float> _FairyShoot2(Enemy fairy)
    {
        // continuous attack
        const int RED_BRANCHES = 6;
        const float GAP = 360f / RED_BRANCHES;
        const int BLUE_BRANCHES = 3;
        const float BLUE_GAP = 360f / BLUE_BRANCHES;
        float angle = 180;
        float subAngle = 90f;
        float ringRadius = 50f;
        for (int i = 0; i < 80; i++)
        {
            if (fairy && !fairy.IsDead())
            {
                float currentRadius = ringRadius * Mathf.Cos(Mathf.PI * i / 60f);
                if (i % 3 == 0)
                {
                    for (int j = 0; j < BLUE_BRANCHES; j++) {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2f, -angle - BLUE_GAP * j, EnemyBulletType.BALL2_BLUE, 5);
                    }
                }
                for (int j = 0; j < RED_BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * (angle + GAP * j)), currentRadius * Mathf.Sin(Mathf.Deg2Rad * (angle + GAP * j))), 1.7f, subAngle - GAP * j, EnemyBulletType.BALL2_DARK_RED, 5);
                }
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(5);
            angle += 19f;
            subAngle -= -7f;
        }
    }

    private IEnumerator<float> _FairyShoot3(Enemy fairy)
    {
        // continuous attack
        const int RED_BRANCHES = 6;
        const float GAP = 360f / RED_BRANCHES;
        const int BLUE_BRANCHES = 3;
        const float BLUE_GAP = 360f / BLUE_BRANCHES;
        float angle = 180;
        float subAngle = 90f;
        float ringRadius = 50f;
        for (int i = 0; i < 80; i++)
        {
            if (fairy && !fairy.IsDead())
            {
                float currentRadius = ringRadius * Mathf.Cos(Mathf.PI * i / 60f);
                if (i % 3 == 0)
                {
                    for (int j = 0; j < BLUE_BRANCHES; j++) {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2f, -angle - BLUE_GAP * j, EnemyBulletType.BALL2_BLUE, 5);
                    }
                }
                for (int j = 0; j < RED_BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * angle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * angle)), 1.7f, subAngle - GAP * j, EnemyBulletType.BALL2_DARK_RED, 5);
                }
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(5);
            angle += 19f;
            subAngle -= -7f;
        }
    }

    private IEnumerator<float> _FairyShoot4(Enemy fairy)
    {
        // continuous attack
        const int RED_BRANCHES = 6;
        const float GAP = 360f / RED_BRANCHES;
        const int BLUE_BRANCHES = 3;
        const float BLUE_GAP = 360f / BLUE_BRANCHES;
        float angle = 0;
        float ringRadius = 15f;
        int rotateDir = 1;
        const float MAX_ANGULAR_VEL = 120f;
        float angularVel = 0.7f;
        const float ANGULAR_ACCEL = 1.2f;
        for (int i = 0; i < 120; i++)
        {
            if (fairy && !fairy.IsDead())
            {
                float currentRadius = -ringRadius * Mathf.Cos(Mathf.PI * i / 35f);
                if (i % 3 == 0)
                {
                    for (int j = 0; j < BLUE_BRANCHES; j++) {
                        ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position, 2f, -angle - BLUE_GAP * j, EnemyBulletType.BALL2_BLUE, 5);
                    }
                }
                for (int j = 0; j < RED_BRANCHES; j++)
                {
                    ECSEntitySpawner.SpawnEnemyBulletE1(fairy.transform.position + new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * (angle + GAP * j)), currentRadius * Mathf.Sin(Mathf.Deg2Rad * (angle + GAP * j))), 2f, angle + GAP * j, EnemyBulletType.BALL2_DARK_RED, 5);
                }
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.15f);
            }
            yield return WaitForFrames.WaitWrapper(5);
            if (rotateDir == 1 && angularVel > MAX_ANGULAR_VEL) {
                rotateDir = -1;
            }
            if (rotateDir == -1 && angularVel < -MAX_ANGULAR_VEL) {
                rotateDir = 1;
            }
            angularVel += rotateDir * ANGULAR_ACCEL;
            angle += angularVel;
        }
    }
}

