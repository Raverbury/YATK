using UnityEngine;

public class Persister : KeepMonoSingleton<Persister>
{
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}