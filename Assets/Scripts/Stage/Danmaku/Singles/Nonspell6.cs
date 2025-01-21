using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;

public class Nonspell6 : AbstractSingle
{
    public int GetHP()
    {
        return 8000;
    }

    public override string GetName()
    {
        return "Light Sign [Divine Intervention]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 42;
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
            new ItemStack(ItemType.BIG_POWER_ITEM, 1),
        });
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        float[] xPositions = { 30, 111, 192, 273, 354 };
        int i = 0;
        int iVel = 1;
        int wait = 55;
        int count = xPositions.Count();
        while (true)
        {
            SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_TAN00, 0.2f);
            float randomX = Random.Range(-20f, 20f);
            Entity blade = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP, 2.5f, 270f, EnemyBulletType.ARROW_YELLOW, 30);
            Entity blade2 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 13f, 2.5f, 270f, EnemyBulletType.ARROW_YELLOW, 30);
            Entity blade3 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 26f, 2.5f, 270f, EnemyBulletType.ARROW_YELLOW, 30);
            Entity blade4 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 39f, 2.5f, 270f, EnemyBulletType.ARROW_YELLOW, 30);
            Entity blade5 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 52f, 2.5f, 270f, EnemyBulletType.ICE_YELLOW, 30);
            Entity blade6 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 65f, 2.5f, 270f, EnemyBulletType.ICE_YELLOW, 30);
            Entity blade7 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 78f, 2.5f, 270f, EnemyBulletType.BALL2_YELLOW, 30);
            Entity hilt1 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX + 10f, Constant.GAME_BORDER_TOP + 52f, 2.5f, 270f, EnemyBulletType.AMULET_YELLOW, 30);
            Entity hilt2 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX - 10f, Constant.GAME_BORDER_TOP + 52f, 2.5f, 270f, EnemyBulletType.AMULET_YELLOW, 30);
            Entity hilt3 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX + 16f, Constant.GAME_BORDER_TOP + 45f, 2.5f, 270f, EnemyBulletType.AMULET_YELLOW, 30);
            Entity hilt4 = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX - 16f, Constant.GAME_BORDER_TOP + 45f, 2.5f, 270f, EnemyBulletType.AMULET_YELLOW, 30);
            // Entity entity = ECSEntitySpawner.SpawnEnemyBulletE1(xPositions[i] + randomX, Constant.GAME_BORDER_TOP + 80f, 2.5f, 270f, EnemyBulletType.BUBBLE_DARK_YELLOW, 30);
            // if (bubbleBulletEntity.TryGetComponent(out EnemyBullet enemyBullet)) {
            //     enemyBullet.HitScreenEdgeCallback = Bounce;
            // }
            CoroutineUtil.RunEntityBoundCoroutine(_SpawnFromBubble(blade), blade);
            if (i == count - 1)
            {
                iVel = -1;
                wait = Mathf.Max(10, wait - 2);
            }
            else if (i == 0)
            {
                iVel = 1;
                wait = Mathf.Max(10, wait - 2);
            }
            i += iVel;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(WaitForFrames.Wait(wait)));
        }
    }

    private IEnumerator<float> _SpawnFromBubble(Entity bubbleBulletEntity)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return WaitForFrames.WaitWrapper(Random.Range(25, 50));
        int branches = Random.Range(2, 8);
        float rot = 360f / branches;
        int waitLeft = 0;
        while (!ECSEntitySpawner.EntityIsDisabled(bubbleBulletEntity))
        {
            LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bubbleBulletEntity);
            if (bulletTransform.Position.y <= STG.Constant.GAME_BORDER_BOTTOM)
            {
                const int EARTH_BRANCHES = 4;
                float spread = 180f / Mathf.Max(1, EARTH_BRANCHES - 1);
                SFXPlayer.RequestPlaySound(RuntimeGameData.Registry.SFX_TAN01, 0.15f);
                for (int i = 0; i < EARTH_BRANCHES; i++)
                {
                    Entity earthBulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletTransform.Position.x, bulletTransform.Position.y, 1.2f, spread * i, EnemyBulletType.BALL2_GREEN, 20);
                    Entity earthBulletEntity2 = ECSEntitySpawner.SpawnEnemyBulletE1(bulletTransform.Position.x, bulletTransform.Position.y, 0.8f, spread * i, EnemyBulletType.BALL2_DARK_GREEN, 20);
                    // CoroutineUtil.RunEntityBoundCoroutine(_AccelerateBullet(earthBulletEntity), earthBulletEntity);
                    // CoroutineUtil.RunEntityBoundCoroutine(_AccelerateBullet(earthBulletEntity2), earthBulletEntity2);
                }
                yield break;
            }
            float r = Random.Range(-20f, 20f);
            if (waitLeft <= 0)
            {
                for (int i = 0; i < branches; i++)
                {
                    Entity subBulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(bulletTransform.Position.x, bulletTransform.Position.y, 0f, r + rot * i, EnemyBulletType.AMULET_RED, 30);
                    CoroutineUtil.RunEntityBoundCoroutine(_AccelerateBullet(subBulletEntity), subBulletEntity);
                }
                waitLeft = Random.Range(100, 260);
            }
            waitLeft -= 1;
            yield return 1;
        }
    }

    private IEnumerator<float> _AccelerateBullet(Entity subBulletEntity)
    {
        yield return WaitForFrames.WaitWrapper(60);

        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 1.2f);
        yield return WaitForFrames.WaitWrapper(15);
        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 0f);
        yield return WaitForFrames.WaitWrapper(60);
        ECSEntitySpawner.SetBulletSpeed(subBulletEntity, 0f);
        float speed = 0f;
        SFXPlayer.RequestPlaySound.Invoke(RuntimeGameData.Registry.SFX_KIRA00, 0.2f);
        while (!ECSEntitySpawner.EntityIsDisabled(subBulletEntity))
        {
            speed = Mathf.Min(speed + 0.1f, 2f);
            ECSEntitySpawner.SetBulletSpeed(subBulletEntity, -speed);
            yield return Timing.WaitForOneFrame;
        }
    }
}
