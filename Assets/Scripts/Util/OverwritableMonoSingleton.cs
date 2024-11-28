using UnityEngine;

public class OverwritableMonoSingleton<T> : MonoBehaviour where T : OverwritableMonoSingleton<T>
{
    public static T instance = null;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}

// NOTE: old prototype idea
// using UnityEngine;

// public class SelfCreatingMonoSingleton<T> : MonoBehaviour where T : SelfCreatingMonoSingleton<T>
// {
//     private T _instance = null;
//     private T Instance
//     {
//         set
//         {
//             if (_instance != null && _instance != value)
//             {
//                 Destroy(this);
//             }
//             _instance = value;
//         }
//         get
//         {
//             if (_instance == null)
//             {
//                 Instance = new GameObject(typeof(T).Name).AddComponent<T>();
//             }
//             return _instance;
//         }
//     }

//     private void Awake()
//     {
//         Instance = (T)this;
//         AltAwake();
//     }

//     protected virtual void AltAwake() { }
// }