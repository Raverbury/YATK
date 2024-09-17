using System;
using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ShotSheet))]
[RequireComponent(typeof(BulletPool))]
public class StageManager : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private BulletPool bulletPool;
    [SerializeField]
    private GameObject enemyPrefab;
    private static StageManager instance = null;

    public static UnityAction<bool> ClearBullet;

    private Dictionary<string, GameObject> enemies = new();

    [SerializeField]
    private List<Type> singles = new() { typeof(Pattern01), typeof(Nonspell2), typeof(Nonspell3), typeof(Nonspell4) };
    public AbstractSingle activeSingle = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartNextAvailableSingle();
    }

    private void OnValidate()
    {
        bulletPool = GetComponent<BulletPool>();
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

    public static GameObject SpawnBulletA1(GameObject gameObject, float speed, float angle, EnemyBulletType bulletType, int delay)
    {
        return SpawnBulletA1(gameObject.transform.position.x, gameObject.transform.position.y, speed, angle, bulletType, delay);
    }

    public static GameObject SpawnBulletA1(Vector2 position, float speed, float angle, EnemyBulletType bulletType, int delay)
    {
        return SpawnBulletA1(position.x, position.y, speed, angle, bulletType, delay);
    }

    public static GameObject SpawnBulletA1(float x, float y, float speed, float angle, EnemyBulletType bulletType, int delay)
    {
        GameObject bullet = instance.bulletPool.RequestBullet();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        enemyBullet.speed = speed;
        (Sprite realSprite, float realSize, float realHitboxRadius, Sprite spawnCloudSprite, float spawnCloudSize, float spawnCloudHitboxRadius) = ShotSheet.GetEnemyBulletData((int)bulletType);
        enemyBullet.SetGraphic(spawnCloudSprite, spawnCloudSize, spawnCloudHitboxRadius);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, enemyBullet, realSprite, realSize, realHitboxRadius, delay), "enemyBulletSpawning");
        return bullet;
    }

    private static IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, EnemyBullet enemyBullet, Sprite realSprite, float realSize, float realHitboxRadius, int delay)
    {
        bullet.SetActive(true);
        enemyBullet.enabled = false;

        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        enemyBullet.SetGraphic(realSprite, realSize, realHitboxRadius);
        enemyBullet.enabled = true;
        bullet.SetActive(true);
    }
}