using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 0)]
public class EnemyData : ScriptableObject
{
    [Header("Metadata")]
    #region metadata
    [Tooltip("The name of this enemy.")]
    public string enemyName;
    [Tooltip("The color of this enemy, should be thematicaly in line with their character.")]
    public Color nameColor;
    #endregion

    [Header("Animations")]
    #region anims
    [Tooltip("The enemy's animation when facing forward/idle.")]
    public AnimationClip idleAnimation;

    [Tooltip("The enemy's animation when moving sideway.")]
    public AnimationClip sideAnimation;

    [Tooltip("The enemy's animation when attacking.")]
    public AnimationClip attackAnimation;
    #endregion

    [Header("Background")]
    #region background
    [Tooltip("The enemy's background image during a spellcard.")]
    public Sprite enemySpellcardBackgroundImage;

    [Tooltip("The enemy's background effect during a spellcard.")]
    public Sprite enemySpellcardBackgroundEffect;
    #endregion
}