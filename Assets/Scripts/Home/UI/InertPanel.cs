using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InertPanel : AbstractHomeSelector
{
    private bool freezeInput = false;

    private void Update()
    {
        if (freezeInput)
        {
            return;
        }
        else if (Input.GetButtonDown("Bomb") || Input.GetButtonDown("Pause"))
        {
            SFXPlayer.RequestPlaySoundWithPause?.Invoke(RuntimeGameData.Registry.SFX_CANCEL, 0.8f);
            HomeManager.EVCancel?.Invoke();
        }
    }
}