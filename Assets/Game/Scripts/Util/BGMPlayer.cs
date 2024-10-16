using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource originalAudioSource;

    public LoopableBGM BGM_HOME;
    public LoopableBGM BGM_STAGE;
    public LoopableBGM BGM_GAMEOVER;

    private LoopableBGM activeBgm = null;

    public static UnityAction RequestPlayHomeBGM;
    public static UnityAction RequestPlayStageBGM;
    public static UnityAction RequestPlayGameoverBGM;

    public static UnityAction RequestStopBGM;

    private void OnEnable()
    {
        StageManager.SetPause += OnPause;

        RequestPlayHomeBGM += PlayHomeBGM;
        RequestPlayStageBGM += PlayStageBGM;
        RequestPlayGameoverBGM += PlayGameoverBGM;

        RequestStopBGM += StopBGM;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;

        RequestPlayHomeBGM -= PlayHomeBGM;
        RequestPlayStageBGM -= PlayStageBGM;
        RequestPlayGameoverBGM -= PlayGameoverBGM;

        RequestStopBGM += StopBGM;
    }

    private void StopBGM()
    {
        originalAudioSource.Stop();
    }

    private void PlayGameoverBGM()
    {
        PlayBGM(BGM_GAMEOVER);
    }

    private void PlayStageBGM()
    {
        PlayBGM(BGM_STAGE);
    }

    private void PlayHomeBGM()
    {
        PlayBGM(BGM_HOME);
    }

    private void PlayBGM(LoopableBGM loopableBGM)
    {
        originalAudioSource.clip = loopableBGM.bgmClip;
        activeBgm = loopableBGM;
        originalAudioSource.Play();
    }

    private void Update()
    {
        if (originalAudioSource.isPlaying && activeBgm != null)
        {
            if (originalAudioSource.timeSamples >= activeBgm.loopEndPoint) {
                originalAudioSource.timeSamples = activeBgm.loopStartPoint;
            }
        }
    }

    private void OnPause(bool isPaused)
    {
        if (isPaused)
        {
            originalAudioSource.Pause();
        }
        else
        {
            originalAudioSource.Play();
        }
    }
}
