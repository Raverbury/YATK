using System.Collections.Generic;
using MEC;
using UnityEngine;

[DisallowMultipleComponent]
public class Pattern01 : MonoBehaviour, ISingle
{
    public int GetLife()
    {
        return 2000;
    }

    public string GetName()
    {
        return "Nonspell 1";
    }

    public int GetScore()
    {
        return 0;
    }

    public int GetTimer()
    {
        return 40;
    }

    public bool IsTimeout()
    {
        return false;
    }

    private void Start()
    {
        Timing.RunCoroutine(_Loop());
    }

    IEnumerator<float> _Loop()
    {
        Enemy enemy;
        if (StageManager.SpawnNamedEnemy(out GameObject enemyGameObject, -100, 100, "mokou"))
        {
            enemy = enemyGameObject.GetComponent<Enemy>();
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(Enemy.MoveEnemyToOver(enemy, new Vector2(192, -90), 60)));
        }
        enemy = enemyGameObject.GetComponent<Enemy>();

        for (int __delay = 0; __delay < 30; __delay++)
        {
            yield return 1;
        }
        enemy.SetAnimState(Enemy.AnimState.Attack);
        for (int __delay = 0; __delay < 30; __delay++)
        {
            yield return 1;
        }
        enemy.MaxHP = GetLife();

        const int BRANCHES = 7;
        const float HALF = (float)BRANCHES / 2f;
        int rotation = 0;
        while (!enemy.IsDead())
        {
            for (int __delay = 0; __delay < 8; __delay++)
            {
                yield return Timing.WaitForOneFrame;
            }
            float r = Random.Range(0, 10);
            for (int i = 0; i < BRANCHES; i++)
            {
                // GameObject bullet = pool.SpawnBulletA1(192, -60, 2 + 3 * ((-1 * Mathf.Abs(i - BRANCHES / 2)) + BRANCHES / 2) / (BRANCHES / 2), 360f / (BRANCHES / 2) * i + r, 0.5f);

                float j = i - HALF;
                j = Mathf.Abs(j);
                j *= -1;
                j += HALF;
                j = Mathf.Max(j, 1);
                float speed = 2 + 6f * (float)j / HALF;
                float angle = 360f / BRANCHES * i + rotation;
                GameObject bullet = StageManager.SpawnBulletA1(enemyGameObject, speed, angle, STG.EnemyBulletType.ARROW_DARK_BLUE, 30);
                Timing.RunCoroutine(_Manipulate(bullet));
                GameObject bullet2 = StageManager.SpawnBulletA1(enemyGameObject, speed, -angle, STG.EnemyBulletType.ARROW_DARK_GREEN, 30);
                Timing.RunCoroutine(_Manipulate(bullet2));
            }
            rotation += 41;
        }
    }

    IEnumerator<float> _Manipulate(GameObject bullet)
    {
        for (int __delay = 0; __delay < 15; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        while (bullet.activeInHierarchy && Vector2.Distance(bullet.transform.position, new Vector2(192, -60)) < 260)
        {
            yield return Timing.WaitForOneFrame;
        }

        enemyBullet.speed = 1.6f;
    }
}
