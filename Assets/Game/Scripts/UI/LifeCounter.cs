using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class LifeCounter : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;
    public const string LIFE_TEXT = "<sprite name=\"life\">";

    private void OnValidate()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Player.PlayerSetLife += UpdateLifeCounter;
    }

    private void OnDisable()
    {
        Player.PlayerSetLife -= UpdateLifeCounter;
    }

    private void UpdateLifeCounter(int life = 2)
    {
        life = Mathf.Clamp(life, 0, 8);
        text.text = "";
        for (int i = 0; i < life; i++)
        {
            text.text += LIFE_TEXT;
        }
    }
}
