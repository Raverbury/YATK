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
            SFXPlayer.EVPlayCancelSound?.Invoke();
            HomeManager.EVCancel?.Invoke();
        }
    }
}