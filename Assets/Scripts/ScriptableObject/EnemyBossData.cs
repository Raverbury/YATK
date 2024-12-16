using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBossData", menuName = "ScriptableObjects/EnemyBossData")]
public class EnemyBossData : EnemyData
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