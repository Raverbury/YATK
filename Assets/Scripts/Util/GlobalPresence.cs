using UnityEngine;

public class GlobalPresence : MonoBehaviour
{
    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}