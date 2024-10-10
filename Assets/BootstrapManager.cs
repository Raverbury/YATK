using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
#else
        Application.targetFrameRate = 60;
#endif
    }

    // Start is called before the first frame update
    private void Start()
    {
        SceneUtil.LoadSceneAsync("Home");
    }
}
