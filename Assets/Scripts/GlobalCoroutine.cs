using System.Collections;
using UnityEngine;

public class GlobalCoroutine : MonoBehaviour
{
    private static GlobalCoroutine _instance = null;

    private static GlobalCoroutine Instance
    {
        get
        {
            if (null == _instance)
            {
                GameObject holder = Object.Instantiate(new GameObject());
                _instance = holder.AddComponent<GlobalCoroutine>();
            }
            return _instance;
        }
    }

    private GlobalCoroutine()
    {

    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }
}
