using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BombCounter : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;
    public const string LIFE_TEXT = "<sprite name=\"bomb\">";

    private void OnValidate()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Player.PlayerSetBomb += UpdateBombCounter;
    }

    private void OnDisable()
    {
        Player.PlayerSetBomb -= UpdateBombCounter;
    }

    private void UpdateBombCounter(int bomb = 2)
    {
        bomb = Mathf.Clamp(bomb, 0, 8);
        text.text = "";
        for (int i = 0; i < bomb; i++)
        {
            text.text += LIFE_TEXT;
        }
    }
}
