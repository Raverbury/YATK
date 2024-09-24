using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    public static StageManager instance = null;

    public static UnityAction<bool> ClearBullet;

    public static UnityAction<bool> SetPause;

    private Dictionary<string, GameObject> enemies = new();

    [SerializeField]
    private List<Type> singles = new() {
        // typeof(Pattern01),
        // typeof(Nonspell2),
        // typeof(Nonspell3),
        // typeof(Nonspell4),
        // typeof(Nonspell5),
    };
    public AbstractSingle activeSingle = null;

    public static bool isPaused = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    private void Start()
    {
        StartNextAvailableSingle();
    }

    private void OnValidate()
    {

    }

    private void OnEnable()
    {
        ClearBullet += KillBulletSpawningCoroutines;
        AbstractSingle.SingleFinish += StartNextAvailableSingle;
    }

    private void OnDisable()
    {
        ClearBullet -= KillBulletSpawningCoroutines;
        AbstractSingle.SingleFinish -= StartNextAvailableSingle;
    }

    private void KillBulletSpawningCoroutines(bool _)
    {
        // Timing.KillCoroutines("enemyBulletSpawning");
    }

    private void StartNextAvailableSingle()
    {
        if (activeSingle != null)
        {
            DestroyImmediate(activeSingle);
            activeSingle = null;
        }
        if (singles.Count == 0)
        {
            return;
        }
        Type singleType = singles[0];
        activeSingle = (AbstractSingle)gameObject.AddComponent(singleType);
        singles.RemoveAt(0);
    }

    public static bool DestroyNamedEnemy(string name)
    {
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
        return enemies.First().Value;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    public static void TogglePause()
    {
        isPaused = !isPaused;
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
}