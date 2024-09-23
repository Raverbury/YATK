using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public AudioClip SFX_PLAYER_MISS;
    public AudioClip SFX_PLAYER_POWERUP;
    public AudioClip SFX_PLAYER_SHOOT;
    public AudioClip SFX_SPELL_START;
    public AudioClip SFX_EXPLODE;
    public AudioClip SFX_ITEM_0;
    public AudioClip SFX_ITEM_1;
    public AudioClip SFX_PLAYER_GRAZE;
    public AudioClip SFX_TIMEOUT_0;
    public AudioClip SFX_TIMEOUT_1;

    public AudioClip SFX_PAUSE;
    public AudioClip SFX_CONFIRM;
    public AudioClip SFX_SELECT;
    public AudioClip SFX_CANCEL;

    private int internalIndex = 0;

    private Dictionary<AudioClip, ushort> audioWaitMap = new();

    public static UnityAction EVPlayConfirmSound;
    public static UnityAction EVPlaySelectSound;
    public static UnityAction EVPlayCancelSound;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < 63; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = originalAudioSource.volume;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.playOnAwake = false;
            audioSources.Add(audioSource);
        }
        for (int i = 0; i < 8; i++)
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

        Player.PlayerSetMiss += PlayMissSound;
        Player.PlayerPowerUp += PlayPowerUpSound;
        AbstractSingle.PatternStart += PlaySpellStartSound;
        Player.PlayerShoot += PlayPlayerShootSound;
        AbstractSingle.SingleExplode += PlaySingleExplodeSound;
        Player.PlayerCollectItem += PlayGenericItemCollectSound;
        Player.EVPlayerGraze += PlayGrazeSound;
        AbstractSingle.PatternTimerSecondTick += PlayTimeoutSound;
        EVPlayCancelSound += PlayCancelSound;
        EVPlayConfirmSound += PlayConfirmSound;
        EVPlaySelectSound += PlaySelectSound;
    }

    private void OnDisable()
    {
        StageManager.SetPause -= OnPause;

        Player.PlayerSetMiss -= PlayMissSound;
        Player.PlayerPowerUp -= PlayPowerUpSound;
        AbstractSingle.PatternStart -= PlaySpellStartSound;
        Player.PlayerShoot -= PlayPlayerShootSound;
        AbstractSingle.SingleExplode -= PlaySingleExplodeSound;
        Player.PlayerCollectItem -= PlayGenericItemCollectSound;
        Player.EVPlayerGraze -= PlayGrazeSound;
        AbstractSingle.PatternTimerSecondTick -= PlayTimeoutSound;
        EVPlayCancelSound -= PlayCancelSound;
        EVPlayConfirmSound -= PlayConfirmSound;
        EVPlaySelectSound -= PlaySelectSound;
    }

    private void PlaySelectSound()
    {
        PlayAudioAlt(SFX_SELECT);
    }

    private void PlayConfirmSound()
    {
        PlayAudioAlt(SFX_CONFIRM);
    }

    private void PlayCancelSound()
    {
        PlayAudioAlt(SFX_CANCEL);
    }

    private void PlayTimeoutSound(ushort secondsLeft)
    {
        if (secondsLeft < 11)
        {
            PlayAudio(SFX_TIMEOUT_1);
        }
    }

    private void PlayMissSound()
    {
        PlayAudio(SFX_PLAYER_MISS);
    }

    private void PlayPowerUpSound()
    {
        PlayAudio(SFX_PLAYER_POWERUP);
    }

    private void PlaySpellStartSound()
    {
        PlayAudio(SFX_SPELL_START);
    }

    private void PlayPlayerShootSound()
    {
        PlayAudio(SFX_PLAYER_SHOOT);
    }

    private void PlaySingleExplodeSound()
    {
        PlayAudio(SFX_EXPLODE);
    }

    private void PlayGenericItemCollectSound()
    {
        PlayAudio(SFX_ITEM_0);
    }

    private void PlayGrazeSound()
    {
        PlayAudio(SFX_PLAYER_GRAZE);
    }

    private void PlayAudio(AudioClip audioClip)
    {
        // foreach (var kvp in audioWaitMap)
        // {
        //     Debug.Log($"{kvp.Key}, {kvp.Value}");
        // }
        if (audioWaitMap.ContainsKey(audioClip) && audioWaitMap[audioClip] > 0)
        {
            return;
        }
        // AudioSource audioSource;
        // int i = 0;
        // do
        // {
        //     if (i >= audioSources.Count)
        //     {
        //         return;
        //     }
        //     audioSource = GetNextAudioSource();
        //     i++;
        // }
        // while (audioSource.isPlaying);
        AudioSource audioSource = GetNextAudioSource(audioSources);
        // Debug.Log($"Playing {audioClip.name}");
        audioSource.clip = audioClip;
        audioSource.Play();
        audioWaitMap[audioClip] = (ushort)(audioClip == SFX_PLAYER_SHOOT ? 9 : 1);
    }

    private void PlayAudioAlt(AudioClip audioClip)
    {
        if (audioWaitMap.ContainsKey(audioClip) && audioWaitMap[audioClip] > 0)
        {
            return;
        }
        AudioSource audioSource = GetNextAudioSource(altAudioSources);
        audioSource.clip = audioClip;
        audioSource.Play();
        audioWaitMap[audioClip] = (ushort)(audioClip == SFX_PLAYER_SHOOT ? 9 : 1);
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
            PlayAudioAlt(SFX_PAUSE);
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
