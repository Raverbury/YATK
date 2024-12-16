using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : OverwritableMonoSingleton<StageManager>
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyMarkerPrefab;
    [SerializeField]
    private GameObject sidebarBottom;

    /// <summary>
    /// Event used to trigger bullet clearing. First bool is for if cleared bullets should drop star items, second bool is if this is a force clear
    /// </summary>
    public static UnityAction<bool, bool> ClearEnemyBullet;
    public static UnityAction<bool> SetPause;
    public static UnityAction EVStageDestroy;
    public static UnityAction RequestPlayerRunOutOfLife;

    private readonly Dictionary<string, GameObject> namedEnemies = new();
    private readonly HashSet<GameObject> allEnemies = new();

    private List<AbstractSingle> stageSingles;

    private AbstractSingle activeSingle = null;

    public static bool isPaused = false;
    private bool shouldRespondToInput = true;
    private int currentSingleIndex = 0;

    private bool isFinished = false;

    protected override void Awake()
    {
        base.Awake();
        TogglePause(false);
        stageSingles = RuntimeGameData.SelectedPatterns;
        RuntimeGameData.EnemyNaturalRewardDropCount = 0;
    }

    private void Start()
    {
        BGMPlayer.RequestPlayStageBGM?.Invoke();
        StartNextAvailableSingle();
    }

    private void OnEnable()
    {
        AbstractSingle.SingleFinish += StartNextAvailableSingle;
        RequestPlayerRunOutOfLife += PlayerRunOutOfLife;
    }

    private void OnDisable()
    {
        AbstractSingle.SingleFinish -= StartNextAvailableSingle;
        RequestPlayerRunOutOfLife -= PlayerRunOutOfLife;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Timing.KillCoroutines();
        EVStageDestroy?.Invoke();
    }

    private void PlayerDefeatAllPatterns()
    {
        isFinished = true;
        PauseMenu.RequestDisableResume?.Invoke(RuntimeGameData.IsPractice ? "Practice Done!" : "All Clear!");
        TogglePause(true);
    }

    private void PlayerRunOutOfLife()
    {
        isFinished = true;
        PauseMenu.RequestDisableResume?.Invoke("Out of Lives!");
        TogglePause(true);
    }

    private void StartNextAvailableSingle()
    {
        if (activeSingle != null)
        {
            activeSingle = null;
        }
        if (stageSingles.Count == 0 || currentSingleIndex >= stageSingles.Count)
        {
            Timing.RunCoroutine(_WaitForGameoverWin());
            return;
        }
        activeSingle = stageSingles[currentSingleIndex];
        activeSingle.StartSingle();
        currentSingleIndex += 1;
    }

    private IEnumerator<float> _WaitForGameoverWin()
    {
        yield return WaitForFrames.WaitWrapper(150);
        PlayerDefeatAllPatterns();
    }

    public static bool DestroyNamedEnemy(string name)
    {
        if (instance.namedEnemies.ContainsKey(name))
        {
            DestroyEnemy(instance.namedEnemies[name].GetComponent<Enemy>());
        }
        return instance.namedEnemies.Remove(name);
    }

    public static bool DestroyEnemy(Enemy enemy)
    {
        bool result = instance.allEnemies.Remove(enemy.gameObject);
        Destroy(enemy.gameObject);
        return result;
    }

    /// <summary>
    /// Spawn a named enemy, return true if that named enemy doesn't already exists
    /// Position is only set for newly spawned enemies (returning true)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool SpawnNamedEnemy(out GameObject enemyGameObject, float x, float y, string name, List<ItemStack> rewards, bool isBossEnemy = true)
    {
        if (instance.namedEnemies.ContainsKey(name))
        {
            enemyGameObject = instance.namedEnemies[name];
            enemyGameObject.GetComponent<Enemy>().InitEnemy(rewards, isBossEnemy);
            return false;
        }
        enemyGameObject = SpawnEnemy(x, y, rewards, isBossEnemy);
        instance.namedEnemies.Add(name, enemyGameObject);
        return true;
    }

    public static GameObject SpawnEnemy(float x, float y, List<ItemStack> rewards, bool isBossEnemy = false)
    {
        GameObject enemyGameObject = Instantiate(instance.enemyPrefab);
        enemyGameObject.transform.position = new Vector3(x, y, 0);
        instance.allEnemies.Add(enemyGameObject);

        if (isBossEnemy)
        {
            GameObject enemyMarkerGO = Instantiate(instance.enemyMarkerPrefab, instance.sidebarBottom.transform);
            enemyMarkerGO.GetComponent<EnemyMarker>().enemy = enemyGameObject.GetComponent<Enemy>();
        }
        enemyGameObject.GetComponent<Enemy>().InitEnemy(rewards, isBossEnemy);
        return enemyGameObject;
    }

    /// <summary>
    /// @Nullable
    /// </summary>
    /// <returns></returns>
    public GameObject GetFirstEnemy()
    {
        if (allEnemies.Count == 0)
        {
            return null;
        }
        try
        {
            return allEnemies.First();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public GameObject[] GetTargetableEnemies()
    {
        if (allEnemies.Count == 0)
        {
            return new GameObject[] { };
        }
        try
        {
            return allEnemies.ToArray();
        }
        catch (InvalidOperationException)
        {
            return new GameObject[] { };
        }
    }

    private void Update()
    {
        if (shouldRespondToInput)
        {
            if (Input.GetButtonDown("Pause"))
            {
                if (!isFinished)
                {
                    if (!isPaused)
                    {
                        SFXPlayer.RequestPlayPauseSound?.Invoke();
                    }
                    TogglePause();
                }
            }
        }
    }

    public static void TogglePause()
    {
        TogglePause(!isPaused);
    }

    public static void TogglePause(bool pause)
    {
        isPaused = pause;
        SetPause?.Invoke(isPaused);
        PausableMono.isPaused = isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            Timing.PauseCoroutines();
        }
        else
        {
            Time.timeScale = 1f;
            Timing.ResumeCoroutines();
        }
    }

    public static void ResolvePause(PauseMenu.PauseResult pauseResult)
    {
        if (instance.shouldRespondToInput)
        {
            switch (pauseResult)
            {
                case PauseMenu.PauseResult.Resume:
                    SFXPlayer.EVPlayConfirmSound?.Invoke();
                    TogglePause();
                    break;
                case PauseMenu.PauseResult.Restart:
                    SFXPlayer.EVPlayConfirmSound?.Invoke();
                    instance.shouldRespondToInput = false;
                    SceneUtil.LoadSceneAsync("Stage");
                    break;
                case PauseMenu.PauseResult.Quit:
                    SFXPlayer.EVPlayCancelSound?.Invoke();
                    instance.shouldRespondToInput = false;
                    SceneUtil.LoadSceneAsync("Home");
                    break;
            }
        }
    }
}