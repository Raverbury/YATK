using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using STG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HomeManager : OverwritableMonoSingleton<HomeManager>
{
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SFXPlayer.EVPlayConfirmSound?.Invoke();
            SceneUtil.LoadSceneAsync("Stage");
            gameObject.SetActive(false);
        }
    }
}