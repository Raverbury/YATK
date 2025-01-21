using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    private readonly List<AudioSource> audioSources = new();
    private readonly List<AudioSource> pausedAudioSources = new();

    private readonly List<AudioSource> altAudioSources = new();

    [SerializeField]
    private AudioSource originalAudioSource;
    [SerializeField]
    private uint numbersToClone = 63;
    [SerializeField]
    private uint numbersToCloneAlt = 8;
    private const float DEFAULT_SFX_VOLUME = 0.15f;

    private int internalIndex = 0;

    private Dictionary<AudioClip, ushort> audioWaitMap = new();

    /// <summary>
    /// Request any available SFXPlayer instance to play an AudioClip at float volume 0~1. This AudioClip is normally active only during non-pause.<br/>
    /// Used for standard game sound effects.
    /// </summary>
    public static UnityAction<AudioClip, float> RequestPlaySound;
    /// <summary>
    /// Request any available SFXPlayer instance to play an AudioClip at float volume 0~1. This AudioClip is normally active only during pause.
    /// Used for navigation sound effects during pause menu or just UI sfxs in general.
    /// </summary>
    public static UnityAction<AudioClip, float> RequestPlaySoundWithPause;


    private void Awake()
    {
        for (int i = 0; i < numbersToClone; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = originalAudioSource.volume;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.playOnAwake = false;
            audioSources.Add(audioSource);
        }
        for (int i = 0; i < numbersToCloneAlt; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = originalAudioSource.volume;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.playOnAwake = false;
            altAudioSources.Add(audioSource);
        }
    }

    private void OnEnable()
    {
        StageManager.SetPause += OnPause;

        RequestPlaySound += PlayAudio;
        RequestPlaySoundWithPause += PlayAudioAlt;

        StageManager.EVStageDestroy += ClearPausedSounds;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;

        RequestPlaySound -= PlayAudio;
        RequestPlaySoundWithPause -= PlayAudioAlt;

        StageManager.EVStageDestroy -= ClearPausedSounds;
    }

    private void ClearPausedSounds()
    {
        pausedAudioSources.Clear();
    }

    private void PlayAudio(AudioClip audioClip, float volume)
    {
        if (audioWaitMap.ContainsKey(audioClip) && audioWaitMap[audioClip] > 0)
        {
            return;
        }
        AudioSource audioSource = GetNextAudioSource(audioSources);
        audioSource.clip = audioClip;
        audioSource.volume = DEFAULT_SFX_VOLUME * volume;
        audioSource.Play();
        audioWaitMap[audioClip] = (ushort)(audioClip == RuntimeGameData.Registry.SFX_PLAYER_SHOOT ? 4 : 1);
    }

    private void PlayAudioAlt(AudioClip audioClip, float volume)
    {
        if (audioWaitMap.ContainsKey(audioClip) && audioWaitMap[audioClip] > 0)
        {
            return;
        }
        AudioSource audioSource = GetNextAudioSource(altAudioSources);
        audioSource.clip = audioClip;
        audioSource.volume = DEFAULT_SFX_VOLUME * volume;
        audioSource.Play();
        audioWaitMap[audioClip] = (ushort)(audioClip == RuntimeGameData.Registry.SFX_PLAYER_SHOOT ? 4 : 3);
    }

    private AudioSource GetNextAudioSource(List<AudioSource> audioSources)
    {
        internalIndex = (internalIndex + 1) % audioSources.Count;
        return audioSources[internalIndex];
    }

    private void Update()
    {
        List<AudioClip> keys = new(audioWaitMap.Keys.ToArray());
        foreach (var key in keys)
        {
            ushort val = audioWaitMap[key];
            audioWaitMap[key] -= 1;
            if (val <= 0)
            {
                audioWaitMap.Remove(key);
            }
        }
    }

    private void OnPause(bool isPaused)
    {
        if (isPaused)
        {
            foreach (var audioSource in audioSources)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                    pausedAudioSources.Add(audioSource);
                }
            }
        }
        else
        {
            foreach (var pausedAudioSource in pausedAudioSources)
            {
                pausedAudioSource.Play();
            }
            pausedAudioSources.Clear();
        }
    }
}
