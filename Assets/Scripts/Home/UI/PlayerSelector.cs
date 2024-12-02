using System.Collections.Generic;
using TMPro;
using UnityEngine;
using STG;
using MEC;
using UnityEngine.UI;
using Assets.Scripts.Util;

public class PlayerSelector : AbstractHomeSelector
{
    [SerializeField]
    private RectTransform playerPanel;
    [SerializeField]
    private TMP_Text playerDescription;
    [SerializeField]
    private List<PlayerData> playerDatas;

    [SerializeField]
    private Image playerSprite;
    [SerializeField]
    private Image playerPowerRatingBar;
    [SerializeField]
    private Image playerSpeedRatingBar;
    [SerializeField]
    private Image playerDifficultyRatingBar;
    [SerializeField]
    private TMP_Text playerStartingLivesCounter;
    [SerializeField]
    private TMP_Text playerStartingBombsCounter;

    private static int currentOption = 0;
    private int keyHeldForFrames = 0;

    private const int PANEL_SWITCH_DURATION = 10;

    private bool freezeInput = false;

    private void Awake()
    {
        SetPlayerDescription(playerDatas[currentOption]);
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
            SelectChoice((currentOption - 1).Modulus(playerDatas.Count), -1);
        }
        else if (Input.GetButton("Left") || Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 15 == 0))
            {
                SelectChoice((currentOption - 1).Modulus(playerDatas.Count), -1);
            }
        }
        else if (Input.GetButtonDown("Right") || Input.GetButtonDown("Down"))
        {
            SelectChoice((currentOption + 1).Modulus(playerDatas.Count), 1);
        }
        else if (Input.GetButton("Right") || Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && keyHeldForFrames % 15 == 0))
            {
                SelectChoice((currentOption + 1).Modulus(playerDatas.Count), 1);
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

    private void SetPlayerDescription(PlayerData playerData)
    {
        playerDescription.text = playerData.playerName;
        playerDescription.color = playerData.nameColor;

        playerSprite.sprite = playerData.sprite;

        playerPowerRatingBar.fillAmount = playerData.RatingPower;
        playerSpeedRatingBar.fillAmount = playerData.RatingSpeed;
        playerDifficultyRatingBar.fillAmount = playerData.RatingDifficulty;
        playerStartingLivesCounter.text = LifeCounter.LIFE_TEXT.Repeat(playerData.initialLife);
        playerStartingBombsCounter.text = BombCounter.BOMB_TEXT.Repeat(playerData.initialBomb);

        // TODO: description
    }

    private IEnumerator<float> _SwitchPanel(int nextOption, int dir)
    {
        freezeInput = true;
        currentOption = nextOption;
        const float ANGLE_CHANGE_VEL = 180f / PANEL_SWITCH_DURATION;
        Vector3 panelRotation = playerPanel.eulerAngles;
        for (int i = 0; i < PANEL_SWITCH_DURATION; i++)
        {
            panelRotation.y += ANGLE_CHANGE_VEL;
            if (i == PANEL_SWITCH_DURATION / 2)
            {
                SetPlayerDescription(playerDatas[currentOption]);
                playerPanel.localScale = new(-playerPanel.localScale.x, 1, 1);
            }
            playerPanel.eulerAngles = panelRotation;
            yield return Timing.WaitForOneFrame;
        }
        freezeInput = false;
    }

    private void ConfirmChoice()
    {
        HomeManager.EVConfirmPlayerSelect?.Invoke(playerDatas[currentOption]);
    }
}