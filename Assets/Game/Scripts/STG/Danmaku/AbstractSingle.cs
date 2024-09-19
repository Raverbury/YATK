
using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractSingle : MonoBehaviour
{
    public abstract int GetTimer();
    public abstract int GetScore();
    public abstract string GetName();
    public abstract bool IsTimeout();

    public static AbstractSingle instance = null;
    public static UnityAction SingleFinish;
    public static UnityAction SingleExplode;
    public static UnityAction PatternStart;

    protected void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        Timing.RunCoroutine(_RunLoop());
    }

    private IEnumerator<float> _RunLoop()
    {
        StageManager.SpawnNamedEnemy(out GameObject enemyGameObject, -100, 100, "mokou");
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.SetEmptyHpCircle();
        Timing.RunCoroutine(_Loop(enemy), "singleLoop");
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(_CheckDone(enemy)));
        enemy.SetAnimState(Enemy.AnimState.Idle);
        CoroutineUtil.KillSingleLoopCRT();
        SingleExplode?.Invoke();
        ScatterItems(enemy.transform.position);
        for (int __delay = 0; __delay < 150; __delay++)
        {
            StageManager.ClearBullet?.Invoke(true);
            yield return 1;
        }
        // if (Player.instance != null)
        // {
        //     Player.instance.Power += 32;
        // }
        SingleFinish?.Invoke();
    }

    private void ScatterItems(Vector2 targetPos)
    {
        int randomNum = 12;
        for (int i = 0; i < randomNum; i++)
        {
            ItemPool.SpawnItemI1(
                targetPos.x + Random.Range(-50f, 50f),
                targetPos.y + Random.Range(-30f, 30f),
                (i % 4) switch
                {
                    3 => ItemType.BIG_POWER_ITEM,
                    _ => ItemType.POWER_ITEM
                });
        }
    }

    protected abstract IEnumerator<float> _Loop(Enemy enemy);

    private IEnumerator<float> _CheckDone(Enemy enemy)
    {
        while (!(enemy.IsDead() && enemy.HasRefilledHP))
        {
            yield return 1;
        }
    }
}
