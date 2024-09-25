using System.Collections.Generic;
using STG;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : MonoBehaviour
{
    public enum PauseResult : int
    {
        Resume = 0,
        Restart = 1,
        Quit = 2,
    }

    [SerializeField]
    private List<TMP_Text> options;

    [SerializeField, HideInInspector]
    private Canvas canvas;

    private int currentOption = 0;

    private int keyHeldForFrames = 0;

    private bool isPaused = false;

    private int colorFrames = 0;
    private int colorChangeVel = 1;
    private const int COLOR_FLUC_DURATION = 30;

    private void OnValidate()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        StageManager.SetPause += OnPause;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;
    }

    private void OnPause(bool isPaused)
    {
        this.isPaused = isPaused;
        canvas.enabled = isPaused;
        currentOption = 0;
    }

    private void Update()
    {
        if (!isPaused)
        {
            return;
        }
        if (Input.GetButtonDown("Shoot"))
        {
            SFXPlayer.EVPlayConfirmSound?.Invoke();
            ConfirmChoice();
        }
        if (Input.GetButtonDown("Restart"))
        {
            SFXPlayer.EVPlayConfirmSound?.Invoke();
            StageManager.ResolvePause(PauseResult.Restart);
        }
        if (Input.GetButtonDown("Quit"))
        {
            SFXPlayer.EVPlayConfirmSound?.Invoke();
            StageManager.ResolvePause(PauseResult.Quit);
        }
        else if (Input.GetButtonDown("Up"))
        {
            SelectChoice((currentOption - 1).Modulus(options.Count));
        }
        else if (Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice((currentOption - 1).Modulus(options.Count));
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            SelectChoice((currentOption + 1).Modulus(options.Count));
        }
        else if (Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice((currentOption + 1).Modulus(options.Count));
            }
        }
        else
        {
            keyHeldForFrames = 0;
        }

        HighlightChoice();
    }

    private void SelectChoice(int nextOption)
    {
        currentOption = nextOption;
        SFXPlayer.EVPlaySelectSound?.Invoke();
    }

    private void HighlightChoice()
    {
        foreach (var text in options)
        {
            text.margin = Vector4.zero;
            text.color = Color.white;
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
        StageManager.ResolvePause((PauseResult)currentOption);
    }
}
