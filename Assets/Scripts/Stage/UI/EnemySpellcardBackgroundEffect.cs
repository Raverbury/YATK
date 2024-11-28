using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EnemySpellcardBackgroundEffect : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Image image;

    public static UnityAction<Sprite> RequestSetBackgroundEffect;

    private void OnValidate()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        RequestSetBackgroundEffect += SetBackground;
    }

    private void OnDisable()
    {
        RequestSetBackgroundEffect -= SetBackground;
    }

    private void SetBackground(Sprite sprite)
    {
        image.enabled = sprite != null;
        if (sprite == null)
        {
            return;
        }
        image.sprite = sprite;
    }
}