using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private List<AudioSource> audioSources;

    public AudioClip SFX_PLAYER_MISS;
    public AudioClip SFX_PLAYER_POWERUP;

    private int internalIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        audioSources = new(GetComponents<AudioSource>());
    }

    private void OnEnable()
    {
        Player.PlayerSetMiss += PlayMissSound;
        Player.PlayerSetPower += PlayPowerUpSound;
    }

    private void OnDisable()
    {
        Player.PlayerSetMiss -= PlayMissSound;
        Player.PlayerSetPower -= PlayPowerUpSound;
    }

    private void PlayMissSound()
    {
        PlayAudio(SFX_PLAYER_MISS);
    }

    private void PlayPowerUpSound(int _)
    {
        PlayAudio(SFX_PLAYER_POWERUP);
    }

    private void PlayAudio(AudioClip audioClip)
    {
        AudioSource audioSource = GetNextAudioSource();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioSource GetNextAudioSource()
    {
        internalIndex = ((internalIndex + 1) >= audioSources.Count) ? 0 : (internalIndex + 1);
        return audioSources[internalIndex];
    }
}
