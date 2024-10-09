using System.Collections;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneUtil.LoadSceneAsync("Home");
    }
}
