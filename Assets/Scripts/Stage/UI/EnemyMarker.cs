using STG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EnemyMarker : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Image image;
    public Enemy enemy = null;
    [Tooltip("The normal enemy marker")]
    public Sprite enemyMarker;
    [Tooltip("The enemy marker when low on HP")]
    public Sprite enemyMarkerNearDeath;

    private void OnValidate()
    {
        image = GetComponent<Image>();
        enemy = GetComponentInParent<Enemy>();
    }

    private void Update()
    {
        if (enemy.IsDestroyed() || enemy == null)
        {
            Destroy(gameObject);
        }
        else
        {
            image.rectTransform.localPosition = new Vector2(enemy.transform.position.x - Constant.GAME_CENTER_X, 0f);
            if (enemy.IsNearDeath())
            {
                image.sprite = enemyMarkerNearDeath;
            }
            else
            {
                image.sprite = enemyMarker;
            }
        }
    }
}
