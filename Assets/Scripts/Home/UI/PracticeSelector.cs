using System.Collections.Generic;
using TMPro;
using UnityEngine;
using STG;
using MEC;

public class PracticeSelector : AbstractHomeSelector
{
    [SerializeField]
    private RectTransform patternPanel;
    [SerializeField]
    private TMP_Text patternName;
    [SerializeField]
    private TMP_Text patternNumber;
    private List<AbstractSingle> availablePatterns;

    private static int currentOption = 0;
    private int keyHeldForFrames = 0;

    private const int PANEL_SWITCH_DURATION = 10;

    private bool freezeInput = false;

    private void Awake()
    {
        availablePatterns = new(DefaultGameData.AllPatterns);
        SetPatternDescription(availablePatterns[currentOption]);
    }

    private void Update()
    {
        if (freezeInput)
        {
            return;
        }
        if (Input.GetButtonDown("Shoot"))
        {
            ConfirmChoice();
        }
        else if (Input.GetButtonDown("Bomb") || Input.GetButtonDown("Pause"))
        {
            SFXPlayer.EVPlayCancelSound?.Invoke();
            HomeManager.EVCancel?.Invoke();
        }
        else if (Input.GetButtonDown("Left") || Input.GetButtonDown("Up"))
        {
            SelectChoice((currentOption - 1).Modulus(availablePatterns.Count), -1);
        }
        else if (Input.GetButton("Left") || Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 15 == 0))
            {
                SelectChoice((currentOption - 1).Modulus(availablePatterns.Count), -1);
            }
        }
        else if (Input.GetButtonDown("Right") || Input.GetButtonDown("Down"))
        {
            SelectChoice((currentOption + 1).Modulus(availablePatterns.Count), 1);
        }
        else if (Input.GetButton("Right") || Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 15 == 0))
            {
                SelectChoice((currentOption + 1).Modulus(availablePatterns.Count), 1);
            }
        }
        else
        {
            keyHeldForFrames = 0;
        }
    }

    private void SelectChoice(int nextOption, int dir)
    {
        SFXPlayer.EVPlaySelectSound?.Invoke();
        Timing.RunCoroutine(_SwitchPanel(nextOption, dir));
    }

    private void SetPatternDescription(AbstractSingle pattern)
    {
        // Debug.Log(playerData.name);
        patternName.text = pattern.GetName();
        patternNumber.text = $"{currentOption + 1}/{availablePatterns.Count}";
    }

    private IEnumerator<float> _SwitchPanel(int nextOption, int dir)
    {
        freezeInput = true;
        currentOption = nextOption;
        const float ANGLE_CHANGE_VEL = 180f / PANEL_SWITCH_DURATION;
        Vector3 panelRotation = patternPanel.eulerAngles;
        for (int i = 0; i < PANEL_SWITCH_DURATION; i++)
        {
            panelRotation.y += ANGLE_CHANGE_VEL;
            if (i == PANEL_SWITCH_DURATION / 2)
            {
                SetPatternDescription(availablePatterns[currentOption]);
                patternPanel.localScale = new(-patternPanel.localScale.x, 1, 1);
            }
            patternPanel.eulerAngles = panelRotation;
            yield return Timing.WaitForOneFrame;
        }
        freezeInput = false;
    }

    private void ConfirmChoice()
    {
        HomeManager.RequestSetPracticePattern?.Invoke(availablePatterns[currentOption]);
        // freezeInput = true;
    }
}