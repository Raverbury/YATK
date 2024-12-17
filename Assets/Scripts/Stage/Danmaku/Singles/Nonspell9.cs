using System.Collections.Generic;
using MEC;
using UnityEngine;
using STG;
using System;
using Unity.Entities;

public class Nonspell9 : AbstractSingle
{
    public int GetHP()
    {
        return 17000;
    }

    public override string GetName()
    {
        return "Illusion [Necker's Dimension]";
    }

    protected override int GetScore()
    {
        return 0;
    }

    protected override int GetTimer()
    {
        return 90;
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
        bool res = enemy && enemy.IsDead();
        if (res) {
            enemy.SetEmptyHpCircle();
        }
        return res;
    }

    private Enemy enemy;

    protected override IEnumerator<float> _Loop()
    {
        enemy = SpawnNamedBossEnemyUtil(ShotSheet.GetBossEnemyData(BossType.MOKOU), new());
        Timing.RunCoroutine(enemy._MoveEnemyToOver(new Vector2(192, -90), 60));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(enemy._RefillHPOver(GetHP(), 60)));
        enemy.SetAnimState(Enemy.AnimState.Attack);
        yield return WaitForFrames.WaitWrapper(30);

        CoroutineUtil.StartSingleLoopCRT(_SpawnCube(new Vector2(Constant.GAME_CENTER_X, Constant.GAME_CENTER_Y), EnemyBulletType.BALL2_BLUE, 4, 180, 0f, 0.7f, 0f));
        yield return WaitForFrames.WaitWrapper(60 * 7);
        CoroutineUtil.StartSingleLoopCRT(_SpawnCube(new Vector2(Constant.GAME_CENTER_X, Constant.GAME_CENTER_Y), EnemyBulletType.BALL2_GREEN, 3, 190, 0.12f, -0.3f, -0.12f));
        yield return WaitForFrames.WaitWrapper(60 * 6);
        CoroutineUtil.StartSingleLoopCRT(_SpawnCube(new Vector2(Constant.GAME_CENTER_X, Constant.GAME_CENTER_Y), EnemyBulletType.BALL2_RED, 4, 195, 0f, 0f, 0.22f));
        yield return WaitForFrames.WaitWrapper(60 * 6);
        CoroutineUtil.StartSingleLoopCRT(_SpawnCube(new Vector2(Constant.GAME_CENTER_X, Constant.GAME_CENTER_Y), EnemyBulletType.BALL2_YELLOW, 7, 170, 0.14f, 0.1f, -0.2f));

        while (true)
        {
            yield return Timing.WaitForOneFrame;
        }
    }

    private IEnumerator<float> _SpawnCube(Vector2 cubeCenter, EnemyBulletType enemyBulletType, int verticesPerEdge, float edgeLength, float xSpinRate = 0.1f, float ySpinRate = 0.2f, float zSpinRate = 0.1f)
    {
        float gapBetweenVertices = edgeLength / (verticesPerEdge - 1);
        float halfLength = edgeLength / 2;
        List<Tuple<FakeTransform, Entity>> vertices = new();
        FakeTransform cubeCenterTransform = new(cubeCenter, Vector3.zero, Vector3.zero);
        for (int y = 0; y < verticesPerEdge; y++)
        {
            float yLevel = -halfLength + y * gapBetweenVertices;
            if (y == 0 || y == verticesPerEdge - 1)
            {
                for (int x = 0; x < verticesPerEdge - 1; x++)
                {
                    Entity bulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(cubeCenter, 0f, 90f, enemyBulletType, 20, false);
                    vertices.Add(new Tuple<FakeTransform, Entity>(new FakeTransform(new Vector3(-halfLength + x * gapBetweenVertices, yLevel, -halfLength), cubeCenterTransform), bulletEntity));
                    CoroutineUtil.KillEntityBoundCoroutines(bulletEntity);
                }
                for (int z = 0; z < verticesPerEdge - 1; z++)
                {
                    Entity bulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(cubeCenter, 0f, 90f, enemyBulletType, 20, false);
                    vertices.Add(new Tuple<FakeTransform, Entity>(new FakeTransform(new Vector3(halfLength, yLevel, -halfLength + z * gapBetweenVertices), cubeCenterTransform), bulletEntity));
                    CoroutineUtil.KillEntityBoundCoroutines(bulletEntity);
                }
                for (int x = 0; x < verticesPerEdge - 1; x++)
                {
                    Entity bulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(cubeCenter, 0f, 90f, enemyBulletType, 20, false);
                    vertices.Add(new Tuple<FakeTransform, Entity>(new FakeTransform(new Vector3(halfLength - x * gapBetweenVertices, yLevel, halfLength), cubeCenterTransform), bulletEntity));
                    CoroutineUtil.KillEntityBoundCoroutines(bulletEntity);
                }
                for (int z = 0; z < verticesPerEdge - 1; z++)
                {
                    Entity bulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(cubeCenter, 0f, 90f, enemyBulletType, 20, false);
                    vertices.Add(new Tuple<FakeTransform, Entity>(new FakeTransform(new Vector3(-halfLength, yLevel, halfLength - z * gapBetweenVertices), cubeCenterTransform), bulletEntity));
                    CoroutineUtil.KillEntityBoundCoroutines(bulletEntity);
                }
            }
            else
            {
                for (int auxX = 0; auxX < 2; auxX++)
                {
                    for (int auxZ = 0; auxZ < 2; auxZ++)
                    {
                        Entity bulletEntity = ECSEntitySpawner.SpawnEnemyBulletE1(cubeCenter, 0f, 90f, enemyBulletType, 20, false);
                        vertices.Add(new Tuple<FakeTransform, Entity>(new FakeTransform(new Vector3(-halfLength + auxX * edgeLength, yLevel, -halfLength + auxZ * edgeLength), cubeCenterTransform), bulletEntity));
                        CoroutineUtil.KillEntityBoundCoroutines(bulletEntity);
                    }
                }
            }
        }
        yield return WaitForFrames.WaitWrapper(20);
        const int FRAMES_TO_EXPAND = 60;
        const float EXPAND_RATE = 1f / FRAMES_TO_EXPAND;
        for (int i = 0; i < FRAMES_TO_EXPAND; i++)
        {
            cubeCenterTransform.scale += EXPAND_RATE * Vector3.one;
            TickFakeTransform(vertices);
            yield return Timing.WaitForOneFrame;
        }
        yield return WaitForFrames.WaitWrapper(90);
        while (true)
        {
            cubeCenterTransform.eulerAngles += new Vector3(xSpinRate, ySpinRate, zSpinRate);
            TickFakeTransform(vertices);
            yield return Timing.WaitForOneFrame;
        }
    }

    private void TickFakeTransform(List<Tuple<FakeTransform, Entity>> vertices)
    {
        foreach (var vertice in vertices)
        {
            vertice.Item1.ApplyTo(vertice.Item2, FakeTransform.ProjectionType.Perspective);
        }
    }
}

