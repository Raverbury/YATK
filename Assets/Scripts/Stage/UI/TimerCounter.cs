using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TimerCounter : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        AbstractSingle.PatternTimerSecondTick += UpdateTimer;
    }

    private void OnDisable()
    {
        AbstractSingle.PatternTimerSecondTick += UpdateTimer;
    }

    private void UpdateTimer(ushort secondsLeft)
    {
        secondsLeft = secondsLeft > 99 ? (ushort)99 : secondsLeft;
        _text.text = secondsLeft.ToString();
    }
}