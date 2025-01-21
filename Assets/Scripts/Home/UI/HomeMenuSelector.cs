using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeMenuSelector : AbstractHomeSelector
{
    public enum HomeMenuResult
    {
        Start = 0,
        Practice = 1,
        Settings = 2,
        Manual = 3,
        Quit = 4,
    }

    private static readonly ChoiceSelector choiceSelector = ChoiceSelector.CreateFrom0(
        (int)HomeMenuResult.Quit,
        // disable quitting in editor/webgl
#if UNITY_WEBGL || UNITY_EDITOR
        new() { (int)HomeMenuResult.Settings, (int)HomeMenuResult.Quit },
#else
        new() {(int)HomeMenuResult.Settings},
#endif
        0);

    [SerializeField]
    private List<TMP_Text> menuOptions;

    private int keyHeldForFrames = 0;

    private int colorFrames = 0;
    private int colorChangeVel = 1;
    private const int COLOR_FLUC_DURATION = 30;

    private void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            ConfirmChoice();
        }
#if !(UNITY_WEBGL || UNITY_EDITOR)
        // disable pressing X to jump to quit choice in editor/webgl
        else if (Input.GetButtonDown("Bomb") || Input.GetButtonDown("Pause"))
        {
            SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_CANCEL, 0.8f);
            choiceSelector.CurrentChoice = (int)HomeMenuResult.Quit;
        }
#endif
        else if (Input.GetButtonDown("Up"))
        {
            SelectChoice(-1);
        }
        else if (Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice(-1);
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            SelectChoice(1);
        }
        else if (Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice(1);
            }
        }
        else
        {
            keyHeldForFrames = 0;
        }

        HighlightChoice();
    }

    private void SelectChoice(int dir)
    {
        if (dir == 1)
        {
            choiceSelector.GetNextChoice();
        }
        else
        {
            choiceSelector.GetPreviousChoice();
        }
        SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_SELECT, 0.8f);
    }

    private void HighlightChoice()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            var text = menuOptions[i];
            text.margin = Vector4.zero;
            if (choiceSelector.IsDisabled(i))
            {
                text.color = Color.gray;
            }
            else
            {
                text.color = Color.white;
            }
        }
        menuOptions[choiceSelector.CurrentChoice].margin = new Vector4(-15, 0, 0, 0);
        float gbColor = (float)colorFrames / COLOR_FLUC_DURATION;
        menuOptions[choiceSelector.CurrentChoice].color = new Vector4(1f, gbColor, gbColor, 1f);
        colorFrames += colorChangeVel;
        if (colorFrames >= COLOR_FLUC_DURATION - 1)
        {
            colorChangeVel = -1;
        }
        else if (colorFrames <= 0)
        {
            colorChangeVel = 1;
        }
    }

    private void ConfirmChoice()
    {
        HomeManager.EVConfirmHomeMenuResult?.Invoke((HomeMenuResult)choiceSelector.CurrentChoice);
    }

}