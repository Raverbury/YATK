using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private AnimationClip frontAnimation;
    public AnimationClip FRONT_ANIMATION
    {
        get
        {
            return frontAnimation;
        }
    }

    [SerializeField]
    private AnimationClip sideAnimation;
    public AnimationClip SIDE_ANIMATION
    {
        get
        {
            return sideAnimation;
        }
    }

    [SerializeField]
    private float unfocusedSpeed;
    public float UNFOCUSED_SPEED
    {
        get
        {
            return unfocusedSpeed;
        }
    }

    [SerializeField]
    private float focusedSpeed;
    public float FOCUSED_SPEED
    {
        get
        {
            return focusedSpeed;
        }
    }

    [SerializeField, Tooltip("The player's hitbox in units (i.e. 1, 2, 3). Default for most should be 5.")]
    private int hitboxRadius;
    public int HITBOX_RADIUS
    {
        get
        {
            return hitboxRadius;
        }
    }

    [SerializeField]
    private int initialLife;
    public int INITIAL_LIFE
    {
        get
        {
            return initialLife;
        }
    }

    [SerializeField]
    private int initialBomb;
    public int INITIAL_BOMB
    {
        get
        {
            return initialBomb;
        }
    }

    [SerializeField, Tooltip("The number of frames to death bomb after getting hit before being considered a miss.")]
    private int deathBombWindow;
    public int DEATH_BOMB_WINDOW
    {
        get
        {
            return initialBomb;
        }
    }

    public string[] methods;

    public void Register(Player player)
    {
        foreach (var methodName in methods) {
            typeof(AbilityList).GetMethod(methodName)?.Invoke(null, new object[] { player });
        }
    }
}