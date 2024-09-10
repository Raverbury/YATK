using System.Collections.Generic;
using MEC;
using UnityEngine;

[DisallowMultipleComponent]
public class Pattern01 : MonoBehaviour
{
    private void Start()
    {
        Timing.RunCoroutine(_Loop());
    }

    IEnumerator<float> _Loop()
    {
        const int BRANCHES = 7;
        const float HALF = (float)BRANCHES / 2f;
        int rotation = 0;
        while (true)
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
                GameObject bullet = StageManager.SpawnBulletA1(192, -60, speed, angle, STG.EnemyBulletType.ARROW_DARK_BLUE, 60);
                Timing.RunCoroutine(_Manipulate(bullet));
                GameObject bullet2 = StageManager.SpawnBulletA1(192, -60, speed, -angle, STG.EnemyBulletType.ARROW_DARK_GREEN, 60);
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
