
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractSingle
{
    protected abstract int GetTimer();
    protected abstract int GetScore();
    public abstract string GetName();
    protected abstract bool IsTimeout();
    protected abstract bool IsSpellCard();
    protected abstract bool IsBossAttack();
    protected virtual void CleanUp() { }

    /// <summary>
    /// Fires after a single ends, after waiting after spellcard and cancelling, used to signal to start next single
    /// </summary>
    public static UnityAction SingleFinish;
    /// <summary>
    /// Fires right when a single ends, before any waiting or cancelling has occured
    /// </summary>
    public static UnityAction SingleExplode;
    public static UnityAction<ushort> PatternTimerSecondTick;
    public static UnityAction<bool> ToggleShowSingleDetails;

    private bool hasTimedOut = false;
    private ushort framesLeft = 0;

    public void StartSingle()
    {
        Timing.RunCoroutine(_RunLoop());
    }

    private IEnumerator<float> _RunLoop()
    {
        // prepare
        ToggleShowSingleDetails?.Invoke(IsBossAttack());
        framesLeft = (ushort)(GetTimer() * 60);
        CoroutineUtil.StartSingleLoopCRT(_Loop());
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(_CheckDone()));
        // clean up
        Vector2 dropPos = new Vector2(192f, -60f);
        CoroutineUtil.KillSingleLoopCRT();
        CleanUp();
        SingleExplode?.Invoke();
        int waitForPatternEnd = IsSpellCard() ? 150 : 5;
        // reset bg
        EnemySpellcardBackgroundImage.RequestSetBackgroundImage(null);
        EnemySpellcardBackgroundEffect.RequestSetBackgroundEffect(null);
        // wait a bit
        for (int __delay = 0; __delay < waitForPatternEnd; __delay++)
        {
            // clear bullets if after boss attack
            if (IsBossAttack())
            {
                StageManager.ClearEnemyBullet?.Invoke(!hasTimedOut, true);
            }
            yield return 1;
        }
        SingleFinish?.Invoke();
    }

    /// <summary>
    /// Coroutine used to define the core logic of a single script. The "What" happens here
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator<float> _Loop();

    /// <summary>
    /// Coroutine used to check if this single script is finished
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> _CheckDone()
    {
        while (true)
        {
            if (SingleIsDoneOutsideOfTimer())
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
                if (IsBossAttack())
                {
                    PatternTimerSecondTick?.Invoke(secondsLeft);
                }
            }
            framesLeft -= 1;
            yield return 1;
        }
    }

    protected abstract bool SingleIsDoneOutsideOfTimer();

    /// <summary>
    /// Util function to create a boss enemy, also handles spell card text + bg stuff
    /// </summary>
    /// <param name="enemyBossData"></param>
    /// <param name="spawnX"></param>
    /// <param name="spawnY"></param>
    /// <returns></returns>
    protected Enemy SpawnNamedBossEnemyUtil(EnemyBossData enemyBossData, List<ItemStack> dropRewards, float spawnX = -100f, float spawnY = 100f)
    {
        StageManager.SpawnNamedEnemy(out GameObject enemyGameObject, spawnX, spawnY, enemyBossData.enemyName, dropRewards);
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.ChangeSprites(enemyBossData);
        if (IsSpellCard())
        {
            EnemySpellcardBackgroundImage.RequestSetBackgroundImage(enemyBossData.enemySpellcardBackgroundImage);
            EnemySpellcardBackgroundEffect.RequestSetBackgroundEffect(enemyBossData.enemySpellcardBackgroundEffect);
            SpellcardName.RequestSetSpellcardName(GetName());
            SFXPlayer.RequestPlaySpellStartSound?.Invoke();
        }
        enemy.SetEmptyHpCircle();
        return enemy;
    }

    protected Enemy SpawnFairyEnemyUtil(EnemyData enemyData, int maxHP, List<ItemStack> dropRewards, float spawnX = -100f, float spawnY = 100f)
    {
        GameObject fairyEnemyGO = StageManager.SpawnEnemy(spawnX, spawnY, dropRewards);
        Enemy enemy = fairyEnemyGO.GetComponent<Enemy>();
        enemy.RefillHP(maxHP);
        enemy.ChangeSprites(enemyData);
        return enemy;
    }
}
