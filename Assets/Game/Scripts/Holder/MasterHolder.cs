using Unity;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ShotSheet))]
[RequireComponent(typeof(AudioSource))]
public class MasterHolder : MonoBehaviour
{
    public static UnityAction PlayerMiss;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() {
        PlayerMiss += PlayDeadSound;
    }

    private void PlayDeadSound() {
        GetComponent<AudioSource>().Play();
    }

    private void OnDisable() {
        PlayerMiss -= PlayDeadSound;
    }
}