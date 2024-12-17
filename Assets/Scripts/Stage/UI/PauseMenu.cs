using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : MonoBehaviour
{
    public enum PauseResult : int
    {
        Resume = 0,
        Restart = 1,
        Quit = 2,
    }

    public static UnityAction<string> RequestDisableResume;

    private readonly ChoiceSelector choiceSelector = ChoiceSelector.CreateFrom0(
        (int)PauseResult.Quit,
        new(),
        0);

    [SerializeField]
    private List<TMP_Text> options;

    [SerializeField]
    private TMP_Text pauseText;

    [SerializeField, HideInInspector]
    private Canvas canvas;

    private int keyHeldForFrames = 0;

    private bool isPaused = false;

    private int colorFrames = 0;
    private int colorChangeVel = 1;
    private const int COLOR_FLUC_DURATION = 30;

    private bool resumeIsDisabled;

    private void OnValidate()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        StageManager.SetPause += OnPause;
        RequestDisableResume += DisableResume;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;
        RequestDisableResume -= DisableResume;
    }

    private void DisableResume(string message)
    {
        choiceSelector.AddDisabledChoices(new() { (int)PauseResult.Resume });
        resumeIsDisabled = true;
        pauseText.text = message;
    }

    private void OnPause(bool isPaused)
    {
        this.isPaused = isPaused;
        canvas.enabled = isPaused;
        choiceSelector.CurrentChoice = resumeIsDisabled ? 1 : 0;
        if (resumeIsDisabled)
        {
            BGMPlayer.RequestPlayBGM?.Invoke(RuntimeGameData.Registry.BGM_NEW_GAMEOVER);
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            return;
        }
        if (Input.GetButtonDown("Shoot"))
        {
            ConfirmChoice();
        }
        if (Input.GetButtonDown("Restart"))
        {
            StageManager.ResolvePause(PauseResult.Restart);
        }
        if (Input.GetButtonDown("Quit"))
        {
            StageManager.ResolvePause(PauseResult.Quit);
        }
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
        SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_SELECT, 1f);
    }

    private void HighlightChoice()
    {
        for (int i = 0; i < options.Count; i++)
        {
            var text = options[i];
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
        options[choiceSelector.CurrentChoice].margin = new Vector4(-15, 0, 0, 0);
        float gbColor = (float)colorFrames / COLOR_FLUC_DURATION;
        options[choiceSelector.CurrentChoice].color = new Vector4(1f, gbColor, gbColor, 1f);
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
        if (resumeIsDisabled && (PauseResult)choiceSelector.CurrentChoice == PauseResult.Resume)
        {
            SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_INVALID, 1f);
            return;
        }
        StageManager.ResolvePause((PauseResult)choiceSelector.CurrentChoice);
    }
}
