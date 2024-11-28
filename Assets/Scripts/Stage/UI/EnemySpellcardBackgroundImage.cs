using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EnemySpellcardBackgroundImage : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Image image;

    public static UnityAction<Sprite> RequestSetBackgroundImage;

    private void OnValidate()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        RequestSetBackgroundImage += SetBackground;
    }

    private void OnDisable()
    {
        RequestSetBackgroundImage -= SetBackground;
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