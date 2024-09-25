using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtil
{
    private const int MIN_LOADING_FRAMES = 120;
    public static void LoadSceneAsync(string sceneName)
    {
        Timing.RunCoroutine(_LoadSceneAsync(sceneName));
    }

    private static IEnumerator<float> _LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;
        LoadingCanvas.EVToggleLoadingCanvas?.Invoke(true);
        int i = 0;
        while (asyncOperation.progress < 0.9f || i < MIN_LOADING_FRAMES)
        {
            yield return Timing.WaitForOneFrame;
            i++;
        }
        asyncOperation.allowSceneActivation = true;
        LoadingCanvas.EVToggleLoadingCanvas?.Invoke(false);
    }
}