using Unity;
using UnityEngine;

[RequireComponent(typeof(ShotSheet))]
public class MasterHolder : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}