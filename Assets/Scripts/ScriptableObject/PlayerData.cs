using STG;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    [Header("Metadata")]
    #region metadata
    [Tooltip("The name of this character.")]
    public string playerName;
    [Tooltip("The color of this character name, should be thematicaly in line with their character.")]
    public Color nameColor;
    [Tooltip("The normal sprite of this character.")]
    public Sprite sprite;
    #endregion

    [Header("Animations")]
    #region anims
    [Tooltip("The player animation when facing forward/idle.")]
    public AnimationClip frontAnimation;

    [Tooltip("The player animation when moving sideway.")]
    public AnimationClip sideAnimation;
    #endregion

    [Header("Stats")]
    #region stats
    [Tooltip("The player's unfocused speed.")]
    public Stat unfocusedSpeed;

    [Tooltip("The player's focused speed.")]
    public Stat focusedSpeed;

    [Tooltip("The player's hitbox in units (i.e. 1, 2, 3). Default for most should be 5.")]
    public Stat hitboxRadius;

    [Tooltip("The player's grazebox in units (i.e. 1, 2, 3). Default for most should be 20.")]
    public Stat grazeboxRadius;

    [Tooltip("The player starts with this many extra lives.")]
    public int initialLife;

    [Tooltip("The player is given this many bombs after spawning/respawning.")]
    public int initialBomb;

    [Tooltip("The player has this many frames to death bomb after getting hit before being considered a miss.")]
    public Stat deathBombWindow;

    [Tooltip("The player's attack power.")]
    public Stat Attack;

    [Tooltip("The player's rate of fire. Frames between shot scaling is dictated by ROFScaling. Default should be 650 if not sure.")]
    // [Range(ROFScaling.MIN_ROF, ROFScaling.MAX_ROF)]
    public Stat RateOfFire;

    [Tooltip("The player's critical hit rate.")]
    public Stat CriticalHitRate;

    [Tooltip("The player's bomb recharge rate.")]
    public Stat BombRechargeRate;

    [Tooltip("The player's point of collection threshold (e.g. 0.1 = 10% away from the top edge).")]
    public Stat ItemCollectionLine;
    #endregion

    #region rating
    [Header("Ratings")]
    [Tooltip("The player's power rating, has no actual use. Should be loosely based on attack.")]
    [Range(0f, 1f)]
    public float RatingPower;

    [Tooltip("The player's speed rating, has no actual use. Should be loosely based on both speeds.")]
    [Range(0f, 1f)]
    public float RatingSpeed;

    [Tooltip("The player's difficulty rating, has no actual use. Should be loosely based on hitbox radius and deathbomb window.")]
    [Range(0f, 1f)]
    public float RatingDifficulty;
    #endregion

    public string[] methods;

    public void Register(Player player)
    {
        foreach (var methodName in methods)
        {
            typeof(AbilityList).GetMethod(methodName)?.Invoke(null, new object[] { player });
        }
    }
}