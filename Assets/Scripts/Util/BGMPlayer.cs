using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource originalAudioSource;

    private LoopableBGM activeBgm = null;

    public static UnityAction<LoopableBGM> RequestPlayBGM;

    public static UnityAction RequestStopBGM;

    private void OnEnable()
    {
        StageManager.SetPause += OnPause;

        RequestPlayBGM += PlayBGM;
        RequestStopBGM += StopBGM;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;

        RequestPlayBGM -= PlayBGM;
        RequestStopBGM -= StopBGM;
    }

    private void StopBGM()
    {
        originalAudioSource.Stop();
    }

    private void PlayBGM(LoopableBGM loopableBGM)
    {
        if (originalAudioSource.clip == loopableBGM.bgmClip) {
            return;
        }
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
