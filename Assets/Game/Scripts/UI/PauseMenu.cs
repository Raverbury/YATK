using System.Collections.Generic;
using STG;
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

    [SerializeField]
    private List<TMP_Text> options;

    [SerializeField]
    private TMP_Text pauseText;

    [SerializeField, HideInInspector]
    private Canvas canvas;

    private int currentOption = 0;

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
        resumeIsDisabled = true;
        pauseText.text = message;
    }

    private void OnPause(bool isPaused)
    {
        this.isPaused = isPaused;
        canvas.enabled = isPaused;
        currentOption = resumeIsDisabled ? 1 : 0;
        if (resumeIsDisabled) {
            BGMPlayer.RequestPlayGameoverBGM?.Invoke();
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
            SelectChoice(CustomModulus(currentOption, -1, options.Count));
        }
        else if (Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice(CustomModulus(currentOption, -1, options.Count));
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            SelectChoice(CustomModulus(currentOption, 1, options.Count));
        }
        else if (Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice(CustomModulus(currentOption, 1, options.Count));
            }
        }
        else
        {
            keyHeldForFrames = 0;
        }

        HighlightChoice();
    }

    private int CustomModulus(int number, int dir, int cap)
    {
        number = (number + dir).Modulus(cap);
        if (resumeIsDisabled && number == 0)
        {
            return (number + dir).Modulus(cap);
        }
        return number;
    }

    private void SelectChoice(int nextOption)
    {
        currentOption = nextOption;
        SFXPlayer.EVPlaySelectSound?.Invoke();
    }

    private void HighlightChoice()
    {
        for (int i = 0; i < options.Count; i++)
        {
            var text = options[i];
            if (i == 0 && resumeIsDisabled)
            {
                text.color = Color.gray;
            }
            else
            {
                text.color = Color.white;
            }
            text.margin = Vector4.zero;
        }
        options[currentOption].margin = new Vector4(-15, 0, 0, 0);
        float gbColor = (float)colorFrames / COLOR_FLUC_DURATION;
        options[currentOption].color = new Vector4(1f, gbColor, gbColor, 1f);
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
        if (resumeIsDisabled && (PauseResult)currentOption == PauseResult.Resume)
        {
            SFXPlayer.RequestPlayInvalidSound?.Invoke();
            return;
        }
        StageManager.ResolvePause((PauseResult)currentOption);
    }
}
