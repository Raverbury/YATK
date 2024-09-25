using UnityEngine;

public class Persister : KeepMonoSingleton<Persister>
{
    protected override void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Purging...");
            Destroy(gameObject);
            return;
        }
        Debug.Log("XD");
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}