using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private List<AudioSource> audioSources;

    public AudioClip SFX_PLAYER_MISS;
    public AudioClip SFX_PLAYER_POWERUP;
    public AudioClip SFX_PLAYER_SHOOT;
    public AudioClip SFX_SPELL_START;
    public AudioClip SFX_EXPLODE;

    private int internalIndex = 0;

    private Dictionary<AudioClip, AudioSource> audioWaitMap = new();

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
        AbstractSingle.PatternStart += PlaySpellStartSound;
        Player.PlayerShoot += PlayPlayerShootSound;
        AbstractSingle.SingleExplode += PlaySingleExplodeSound;
    }

    private void OnDisable()
    {
        Player.PlayerSetMiss -= PlayMissSound;
        Player.PlayerSetPower -= PlayPowerUpSound;
        AbstractSingle.PatternStart -= PlaySpellStartSound;
        Player.PlayerShoot -= PlayPlayerShootSound;
        AbstractSingle.SingleExplode -= PlaySingleExplodeSound;
    }

    private void PlayMissSound()
    {
        PlayAudio(SFX_PLAYER_MISS);
    }

    private void PlayPowerUpSound(int _)
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

    private void PlayAudio(AudioClip audioClip)
    {
        if (audioWaitMap.ContainsKey(audioClip) && audioWaitMap[audioClip].isPlaying)
        {
            return;
        }
        AudioSource audioSource;
        int i = 0;
        do
        {
            if (i >= audioSources.Count)
            {
                return;
            }
            audioSource = GetNextAudioSource();
            i++;
        }
        while (audioSource.isPlaying);
        audioSource.clip = audioClip;
        audioSource.Play();
        audioWaitMap[audioClip] = audioSource;
    }

    private AudioSource GetNextAudioSource()
    {
        internalIndex = ((internalIndex + 1) >= audioSources.Count) ? 0 : (internalIndex + 1);
        return audioSources[internalIndex];
    }

    private void Update()
    {
    }
}
