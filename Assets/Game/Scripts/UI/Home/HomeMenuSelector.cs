using System.Collections.Generic;
using TMPro;
using UnityEngine;
using STG;

public class HomeMenuSelector : AbstractHomeSelector
{
    public enum HomeMenuResult
    {
        Start = 0,
        Settings = 1,
        Quit = 2,
    }

    [SerializeField]
    private List<TMP_Text> menuOptions;
    private int currentOption;

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
        else if (Input.GetButtonDown("Bomb") || Input.GetButtonDown("Pause"))
        {
            SFXPlayer.EVPlayCancelSound?.Invoke();
            currentOption = 2;
        }
        else if (Input.GetButtonDown("Up"))
        {
            SelectChoice((currentOption - 1).Modulus(menuOptions.Count));
        }
        else if (Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice((currentOption - 1).Modulus(menuOptions.Count));
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            SelectChoice((currentOption + 1).Modulus(menuOptions.Count));
        }
        else if (Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 5 == 0))
            {
                SelectChoice((currentOption + 1).Modulus(menuOptions.Count));
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
        foreach (var text in menuOptions)
        {
            text.margin = Vector4.zero;
            text.color = Color.white;
        }
        menuOptions[currentOption].margin = new Vector4(-15, 0, 0, 0);
        float gbColor = (float)colorFrames / COLOR_FLUC_DURATION;
        menuOptions[currentOption].color = new Vector4(1f, gbColor, gbColor, 1f);
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
        HomeManager.EVConfirmHomeMenuResult?.Invoke((HomeMenuResult)currentOption);
    }

}