
using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractSingle
{
    public abstract int GetTimer();
    public abstract int GetScore();
    public abstract string GetName();
    public abstract bool IsTimeout();

    /// <summary>
    /// Fires after a single ends, after waiting after spellcard and cancelling, used to signal to start next single
    /// </summary>
    public static UnityAction SingleFinish;
    /// <summary>
    /// Fires right when a single ends, before any waiting or cancelling has occured
    /// </summary>
    public static UnityAction SingleExplode;
    public static UnityAction<ushort> PatternTimerSecondTick;

    private bool hasTimedOut = false;
    private ushort framesLeft = 0;

    /// <summary>
    /// Bool that controls whether this single is a spellcard, should be set during construction
    /// </summary>
    public bool IsSpellCard = false;

    public void StartSingle(EnemyData enemyData)
    {
        Timing.RunCoroutine(_RunLoop(enemyData));
    }

    /// <summary>
    /// Plays the spellcard sound if this single is a spellcard.
    /// The check here is to not have to decide whether a single is spellcard inside the single script so every script can just call this and not care
    /// </summary>
    private void PlaySpellStartSound()
    {
        if (!IsSpellCard)
        {
            return;
        }
        SFXPlayer.RequestPlaySpellStartSound?.Invoke();
    }

    private IEnumerator<float> _RunLoop(EnemyData enemyData)
    {
        StageManager.SpawnNamedEnemy(out GameObject enemyGameObject, -100, 100, "mokou");
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.ChangeSprites(enemyData);
        if (IsSpellCard)
        {
            EnemySpellcardBackgroundImage.RequestSetBackgroundImage(enemyData.enemySpellcardBackgroundImage);
            EnemySpellcardBackgroundEffect.RequestSetBackgroundEffect(enemyData.enemySpellcardBackgroundEffect);
            SpellcardName.RequestSetSpellcardName(GetName());
        }
        PlaySpellStartSound();
        enemy.SetEmptyHpCircle();
        enemy.IsInvulnerable = IsTimeout();
        framesLeft = (ushort)(GetTimer() * 60);
        Timing.RunCoroutine(_Loop(enemy), "singleLoop");
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(_CheckDone(enemy)));
        Vector2 dropPos = new Vector2(192f, -60f);
        CoroutineUtil.KillSingleLoopCRT();
        if (enemy != null)
        {
            enemy.SetAnimState(Enemy.AnimState.Idle);
            dropPos = enemy.transform.position;
        }
        SingleExplode?.Invoke();
        DropRewards(dropPos);
        int waitForPatternEnd = IsSpellCard ? 150 : 5;
        EnemySpellcardBackgroundImage.RequestSetBackgroundImage(null);
        EnemySpellcardBackgroundEffect.RequestSetBackgroundEffect(null);
        for (int __delay = 0; __delay < waitForPatternEnd; __delay++)
        {
            StageManager.ClearEnemyBullet?.Invoke(!hasTimedOut, true);
            yield return 1;
        }
        // if (Player.instance != null)
        // {
        //     Player.instance.Power += 32;
        // }
        SingleFinish?.Invoke();
    }

    protected virtual void DropRewards(Vector2 targetPos)
    {
        if (!IsSpellCard)
        {
            return;
        }
        int randomNum = 12;
        for (int i = 0; i < randomNum; i++)
        {
            ECSEntitySpawner.SpawnItemI1(
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
        while (true)
        {
            if (enemy.IsDead() && enemy.HasRefilledHP)
            {
                break;
            }
            if (framesLeft <= 0)
            {
                hasTimedOut = true;
                break;
            }
            if (framesLeft % 60 == 0)
            {
                ushort secondsLeft = (ushort)(framesLeft / 60);
                PatternTimerSecondTick?.Invoke((ushort)(framesLeft / 60));
            }
            framesLeft -= 1;
            yield return 1;
        }
    }
}
