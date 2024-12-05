using System;
using System.Collections.Generic;
using STG;
using TMPro;
using UnityEngine;

public class ShotSheet : OverwritableMonoSingleton<ShotSheet>
{
    #region spawn clouds
    [Header("Spawn clouds")]
    [SerializeField]
    private ShotData spawnClouds;
    #endregion

    #region enemy bullet
    [Header("Enemy bullets")]
    [SerializeField]
    private List<ShotData> enemyBullets;
    #endregion

    #region player weapons
    [Header("Player weapons")]
    [SerializeField]
    private List<ShotData> playerWeapons;
    #endregion

    #region items
    [Header("Items")]
    [SerializeField]
    private List<Sprite> items;
    #endregion

    #region bombs
    [Header("Bombs")]
    [SerializeField]
    private List<Sprite> bombs;
    #endregion

    public static (Sprite, float, float, Sprite, float, float) GetEnemyBulletData(EnemyBulletType enemyBulletType)
    {
        int index = (int)enemyBulletType;
        int type = index / 16;
        int color = index % 16;
        int spawnCloudIndex = color;
        try
        {


            ShotData data = instance.enemyBullets[type];
            ShotData spawnClouds = instance.spawnClouds;
            if (data.SPRITES.Count == 16)
            {
                spawnCloudIndex = color switch
                {
                    0 => 0,
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    7 or 8 => 4,
                    9 or 10 => 5,
                    11 or 12 or 13 or 14 => 6,
                    _ => 7,
                };
            }
            else if (data.SPRITES.Count == 4)
            {
                spawnCloudIndex = color switch
                {
                    0 => 1,
                    1 => 3,
                    2 => 5,
                    _ => 6,
                };
            }
            return (data.SPRITES[color], data.size, data.hitboxRadius, spawnClouds.SPRITES[spawnCloudIndex], spawnClouds.size, spawnClouds.hitboxRadius);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new Exception($"ArgumentOutOfRangeException, did you forget to assign a sprite for {enemyBulletType}");
        }
    }

    public static ShotData GetPlayerShotData(PlayerShotType playerShotType)
    {
        try
        {
            return instance.playerWeapons[(int)playerShotType];
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new Exception($"ArgumentOutOfRangeException, did you forget to assign a sprite for {playerShotType}");
        }
    }

    public static Sprite GetItemSprite(ItemType itemType)
    {
        try
        {
            return instance.items[(int)itemType];
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new Exception($"ArgumentOutOfRangeException, did you forget to assign a sprite for {itemType}");
        }
    }

    public static Sprite GetBombSprite(BombType bombType)
    {
        try
        {
            return instance.bombs[(int)bombType];
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new Exception($"ArgumentOutOfRangeException, did you forget to assign a sprite for {bombType}");
        }
    }
}