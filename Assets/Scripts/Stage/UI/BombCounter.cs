using Assets.Scripts.Util;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BombCounter : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;
    public const string BOMB_TEXT = "<sprite name=\"bomb\">";

    private void OnValidate()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Player.EVPlayerSetRemainingBombAmount += UpdateBombCounter;
    }

    private void OnDisable()
    {
        Player.EVPlayerSetRemainingBombAmount -= UpdateBombCounter;
    }

    private void UpdateBombCounter(int bomb = 2)
    {
        bomb = Mathf.Clamp(bomb, 0, 8);
        text.text = BOMB_TEXT.Repeat(bomb);
    }
}
