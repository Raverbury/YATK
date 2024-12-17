using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    public Registry Registry;

    private void Awake()
    {
#if UNITY_EDITOR
#else
        Application.targetFrameRate = 60;
#endif
    }

    private void Start()
    {
        RuntimeGameData.Registry = Instantiate(Registry);
        SceneUtil.LoadSceneAsync("Home");
    }
}
