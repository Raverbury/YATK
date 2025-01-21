using System.Collections.Generic;
using System.Linq;
using MEC;
using STG;
using TMPro;
using UnityEngine;

public class LoadoutSelector : AbstractHomeSelector
{
    [SerializeField]
    private RectTransform loadoutPanel;
    [SerializeField]
    private TMP_Text shotLabel;
    [SerializeField]
    private TMP_Text shotName;
    [SerializeField]
    private TMP_Text shotDescription;
    [SerializeField]
    private TMP_Text bombLabel;
    [SerializeField]
    private TMP_Text bombName;
    [SerializeField]
    private TMP_Text bombDescription;

    private static int currentShotOption = 0;
    private static int currentBombOption = 0;

    private int keyHeldForFrames = 0;

    private const int PANEL_SWITCH_DURATION = 10;

    private bool freezeInput = false;

    private void Awake()
    {
        SetLoadoutDetails(DefaultGameData.AllShots[currentShotOption], DefaultGameData.AllBombs[currentBombOption]);
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
            SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_CANCEL, 0.8f);
            HomeManager.EVCancel?.Invoke();
        }
        // change shot
        else if (Input.GetButtonDown("Left"))
        {
            SelectShotChoice((currentShotOption - 1).Modulus(DefaultGameData.AllShots.Count()));
        }
        else if (Input.GetButton("Left"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && (keyHeldForFrames - 30) % (PANEL_SWITCH_DURATION + 1) == 0))
            {
                SelectShotChoice((currentShotOption - 1).Modulus(DefaultGameData.AllShots.Count()));
            }
        }
        else if (Input.GetButtonDown("Right"))
        {
            SelectShotChoice((currentShotOption + 1).Modulus(DefaultGameData.AllShots.Count()));
        }
        else if (Input.GetButton("Right"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && (keyHeldForFrames - 30) % (PANEL_SWITCH_DURATION + 1) == 0))
            {
                SelectShotChoice((currentShotOption + 1).Modulus(DefaultGameData.AllShots.Count()));
            }
        }
        // change bomb
        else if (Input.GetButtonDown("Up"))
        {
            SelectBombChoice((currentBombOption - 1).Modulus(DefaultGameData.AllBombs.Count()));
        }
        else if (Input.GetButton("Up"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && (keyHeldForFrames - 30) % (PANEL_SWITCH_DURATION + 1) == 0))
            {
                SelectBombChoice((currentBombOption - 1).Modulus(DefaultGameData.AllBombs.Count()));
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            SelectBombChoice((currentBombOption + 1).Modulus(DefaultGameData.AllBombs.Count()));
        }
        else if (Input.GetButton("Down"))
        {
            keyHeldForFrames++;
            if (keyHeldForFrames == 30 || (keyHeldForFrames > 30 && (keyHeldForFrames - 30) % (PANEL_SWITCH_DURATION + 1) == 0))
            {
                SelectBombChoice((currentBombOption + 1).Modulus(DefaultGameData.AllBombs.Count()));
            }
        }
        // end
        else
        {
            keyHeldForFrames = 0;
        }
    }

    private void SelectShotChoice(int nextShotOption)
    {
        SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_SELECT, 0.8f);
        Timing.RunCoroutine(_SwitchPanel(nextShotOption, currentBombOption, RotateDir.Horizontal));
    }

    private void SelectBombChoice(int nextBombOption)
    {
        SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_SELECT, 0.8f);
        Timing.RunCoroutine(_SwitchPanel(currentShotOption, nextBombOption, RotateDir.Vertical));
    }

    private void SetLoadoutDetails(AbstractShot shot, AbstractBombWeapon bomb)
    {
        shotName.text = shot.ShotName();
        shotName.color = shot.ShotColor();
        shotDescription.text = shot.ShotDescription();
        bombName.text = bomb.BombName();
        bombName.color = bomb.BombColor();
        bombDescription.text = bomb.BombDescription();
    }

    private IEnumerator<float> _SwitchPanel(int nextShotOption, int nextBombOption, RotateDir dir)
    {
        freezeInput = true;
        currentShotOption = nextShotOption;
        currentBombOption = nextBombOption;
        const float ANGLE_CHANGE_VEL = 180f / PANEL_SWITCH_DURATION;
        Vector3 panelRotation = loadoutPanel.eulerAngles;
        for (int i = 0; i < PANEL_SWITCH_DURATION; i++)
        {
            if (i == PANEL_SWITCH_DURATION / 2)
            {
                SetLoadoutDetails(DefaultGameData.AllShots[currentShotOption], DefaultGameData.AllBombs[currentBombOption]);
            }
            if (i >= PANEL_SWITCH_DURATION / 2)
            {
                if (dir == RotateDir.Horizontal)
                {
                    panelRotation.y = 180f - ANGLE_CHANGE_VEL * (i + 1);
                }
                else
                {
                    panelRotation.x = 180f - ANGLE_CHANGE_VEL * (i + 1);
                }
            }
            else
            {
                if (dir == RotateDir.Horizontal)
                {
                    panelRotation.y = ANGLE_CHANGE_VEL * (i + 1);
                }
                else
                {
                    panelRotation.x = ANGLE_CHANGE_VEL * (i + 1);
                }
            }
            loadoutPanel.eulerAngles = panelRotation;
            yield return Timing.WaitForOneFrame;
        }
        freezeInput = false;
    }

    private void ConfirmChoice()
    {
        HomeManager.EVConfirmLoadoutSelect?.Invoke(DefaultGameData.AllShots[currentShotOption], DefaultGameData.AllBombs[currentBombOption]);
    }

    private enum RotateDir
    {
        Horizontal = 0,
        Vertical = 1,
    }
}