using UnityEngine;

public abstract class PausableMono : MonoBehaviour
{
    public static bool isPaused = false;

    private void Update()
    {
        if (!isPaused)
        {
            PausableUpdate();
        }
    }

    protected abstract void PausableUpdate();
}