using System;
using System.Collections.Generic;
using STG;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
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

    [Tooltip("The player starts with this many extra lives.")]
    public int initialLife;

    [Tooltip("The player is given this many bombs after spawning/respawning.")]
    public int initialBomb;

    [Tooltip("The player has this many frames to death bomb after getting hit before being considered a miss.")]
    public Stat deathBombWindow;

    [Tooltip("The player's attack power.")]
    public Stat Attack;

    [Tooltip("The player's rate of fire. Time between shot is generally calculated as 5 / (5 + rof).")]
    public Stat RateOfFire;

    [Tooltip("The player's critical hit rate.")]
    public Stat CriticalHitRate;

    [Tooltip("The player's bomb recharge rate.")]
    public Stat BombRechargeRate;

    [Tooltip("The player's point of collection threshold (percent away from the top edge).")]
    public Stat ItemCollectionLine;
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