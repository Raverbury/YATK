using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageManager : OverwritableMonoSingleton<StageManager>
{
    [SerializeField]
    private GameObject enemyPrefab;

    public EnemyData enemyData;

    public static UnityAction<bool, bool> ClearEnemyBullet;
    public static UnityAction<bool> SetPause;
    public static UnityAction EVStageDestroy;

    private Dictionary<string, GameObject> enemies = new();

    private List<AbstractSingle> singles = new() {
        new Nonspell2(),
        new Pattern01(){IsSpellCard = true},

        new MokouNon1(),
        new Nonspell3(){IsSpellCard = true},

        new Nonspell8(),
        new Nonspell4(){IsSpellCard = true},

        new Nonspell10(),
        new StarSpell1(){IsSpellCard = true},

        new Nonspell12(),
        new OldtroxSpell(){IsSpellCard = true},

        new Nonspell11(),
        new Nonspell6(){IsSpellCard = true},

        new Nonspell13(),
        new ShapeSpell(){IsSpellCard = true},

        new Nonspell5(),
        new SurroundSpell1(){IsSpellCard = true},

        new Nonspell7(){IsSpellCard = true},

        new Nonspell9(){IsSpellCard = true},
    };
    private AbstractSingle activeSingle = null;

    public static bool isPaused = false;
    private bool shouldRespondToInput = true;

    protected override void Awake()
    {
        base.Awake();
        TogglePause(false);
    }

    private void Start()
    {
        StartNextAvailableSingle();
    }

    private void OnEnable()
    {
        ClearEnemyBullet += KillBulletSpawningCoroutines;
        AbstractSingle.SingleFinish += StartNextAvailableSingle;
    }

    private void OnDisable()
    {
        ClearEnemyBullet -= KillBulletSpawningCoroutines;
        AbstractSingle.SingleFinish -= StartNextAvailableSingle;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Timing.KillCoroutines();
        EVStageDestroy?.Invoke();
    }

    private void KillBulletSpawningCoroutines(bool _, bool _2)
    {
        // Timing.KillCoroutines("enemyBulletSpawning");
    }

    private void StartNextAvailableSingle()
    {
        if (activeSingle != null)
        {
            activeSingle = null;
        }
        if (singles.Count == 0)
        {
            return;
        }
        activeSingle = singles[0];
        activeSingle.StartSingle(enemyData);
        singles.RemoveAt(0);
    }

    public static bool DestroyNamedEnemy(string name)
    {
        if (instance.enemies.ContainsKey(name))
        {
            Destroy(instance.enemies[name]);
        }
        return instance.enemies.Remove(name);
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
    public static bool SpawnNamedEnemy(out GameObject gameObject, float x, float y, string name)
    {
        if (instance.enemies.ContainsKey(name))
        {
            gameObject = instance.enemies[name];
            return false;
        }
        gameObject = SpawnEnemy(x, y);
        instance.enemies.Add(name, gameObject);
        return true;
    }

    public static GameObject SpawnEnemy(float x, float y)
    {
        GameObject enemyGameObject = Instantiate(instance.enemyPrefab);
        enemyGameObject.transform.position = new Vector3(x, y, 0);
        return enemyGameObject;
    }

    /// <summary>
    /// @Nullable
    /// </summary>
    /// <returns></returns>
    public GameObject GetFirstEnemy()
    {
        if (enemies.Count == 0)
        {
            return null;
        }
        try
        {
            return enemies.First().Value;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public GameObject[] GetTargetableEnemies()
    {
        if (enemies.Count == 0)
        {
            return new GameObject[] { };
        }
        try
        {
            return enemies.Select(kvp => kvp.Value).ToArray();
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
                TogglePause();
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