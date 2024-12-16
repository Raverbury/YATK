using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Animations")]
    #region anims
    [Tooltip("The enemy's animation when facing forward/idle.")]
    public AnimationClip idleAnimation;

    [Tooltip("The enemy's animation when moving sideway.")]
    public AnimationClip sideAnimation;
    #endregion
}