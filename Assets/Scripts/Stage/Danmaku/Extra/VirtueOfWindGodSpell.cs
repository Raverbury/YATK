using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using Unity.Entities;
using Assets.Scripts.Util;

public class VirtueOfWindGodSpell : AbstractSingle
{
    public int GetHP()
    {
        return 23000;
    }

    public override string GetName()
    {
        return "Borrowed Virtue of Phoenix";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 120;
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

    protected override bool SingleIsDoneOutsideOfTimer()
    {
        return enemy && enemy.IsDead();
    }

    protected override void CleanUp()
    {
        if (enemy)
        {
            enemy.SetEmptyHpCircle();
        }
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new(){
            new ItemStack(ItemType.POWER_ITEM, 7),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -160), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        float offset = 90f;
        int wait = 240;

        while (true)
        {
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 30f, 40f, offset, EnemyBulletType.AMULET_RED, 4, 8, 70));
            yield return WaitForFrames.WaitWrapper(35);
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 45f, 60f, offset + 36f, EnemyBulletType.AMULET_BLUE, 4, 8, 70));
            yield return WaitForFrames.WaitWrapper(35);
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 70f, 80f, offset + 72f, EnemyBulletType.AMULET_GREEN, 4, 8, 70));
            yield return WaitForFrames.WaitWrapper(wait);
            offset = Random.Range(0f, 360f);
            wait = Mathf.Max(60, wait - 4);
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 70f, 80f, offset + 72f, EnemyBulletType.AMULET_YELLOW, 4, 8, 140));
            yield return WaitForFrames.WaitWrapper(35);
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 45f, 60f, offset + 36f, EnemyBulletType.AMULET_PURPLE, 4, 8, 70));
            yield return WaitForFrames.WaitWrapper(35);
            CoroutineUtil.StartSingleLoopCRT(_SpawnDVOWGCircles(enemy.transform.position, 5, 30f, 40f, offset, EnemyBulletType.AMULET_SKY, 4, 8, 0));
            yield return WaitForFrames.WaitWrapper(wait);
            offset = Random.Range(0f, 360f);
            wait = Mathf.Max(120, wait - 12);
        }
    }

    private IEnumerator<float> _SpawnDVOWGCircles(Vector2 centerPos, int numOfCircles, float radius, float distance, float startingAngle, EnemyBulletType enemyBulletType, int bulletsPerGroup, int numOfGroups, int extraWait)
    {
        float spreadBetweenCircles = 360f / numOfCircles;

        Vector2 c1Center = centerPos + distance * MathUtil.GetUnitVectorPointingAt(startingAngle);
        Vector2 c2Center = centerPos + distance * MathUtil.GetUnitVectorPointingAt(startingAngle + spreadBetweenCircles);
        Vector2 centerBetweenNeighboringCircles = (c1Center + c2Center) / 2f;
        float distanceFromCBNCToCenter = Vector2.Distance(centerBetweenNeighboringCircles, c1Center);

        float halfOccupiedArc = 0f;

        if (distanceFromCBNCToCenter - radius < 0f)
        {
            float betweenCenterToTrueCenter = (180f - spreadBetweenCircles) / 2f;
            halfOccupiedArc = Mathf.Rad2Deg * Mathf.Acos(distanceFromCBNCToCenter / radius) + betweenCenterToTrueCenter;
        }

        int numOfBullets = numOfGroups * bulletsPerGroup;
        float anglePerStep = (360f - 2 * halfOccupiedArc) / (numOfBullets - 1);
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_LAZER01, 0.25f);
        for (int i = 0; i < numOfBullets; i++)
        {
            for (int j = 0; j < numOfCircles; j++)
            {
                Vector2 circleCenter = centerPos.ExtendBy(distance, startingAngle + spreadBetweenCircles * j);
                float circleStartingAngle = startingAngle + spreadBetweenCircles * j - 180f;
                float spawnAngle = circleStartingAngle + halfOccupiedArc + anglePerStep * i;
                // Entity bullet = ECSEntitySpawner.SpawnEnemyBulletE1(circleCenter.ExtendBy(radius, spawnAngle), 0f, spawnAngle - 180f - 0.8f * anglePerStep * (i % bulletsPerGroup) - 20f * (i / bulletsPerGroup - groupSpreadOffset), enemyBulletType, 1);
                Entity bullet = ECSEntitySpawner.SpawnEnemyBulletE1(circleCenter.ExtendBy(radius, spawnAngle), 0f, spawnAngle - 180f - 2.5f * (i % bulletsPerGroup), enemyBulletType, 1);
                CoroutineUtil.RunEntityBoundCoroutine(_MoveAfter(bullet, numOfBullets - i + 60 + extraWait, 0.45f, 4), bullet);
            }
            yield return WaitForFrames.WaitWrapper(1);
        }
    }

    private IEnumerator<float> _MoveAfter(Entity entity, int waitDuration, float speedGained, int accelerationOverFrames)
    {
        yield return WaitForFrames.WaitWrapper(waitDuration);

        float speed = 0;
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_KIRA00, 0.2f);
        for (int i = 0; i < accelerationOverFrames; i++)
        {
            speed += speedGained;
            ECSEntitySpawner.SetBulletSpeed(entity, speed);
        }
    }
}

