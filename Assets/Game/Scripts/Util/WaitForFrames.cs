using System.Collections.Generic;
using MEC;
using UnityEngine;

class WaitForFrames
{
    private WaitForFrames()
    {

    }

    public static IEnumerator<float> Wait(int framesToWait)
    {
        for (int __delay = 0; __delay < framesToWait; __delay++)
        {
            yield return 1;
        }
    }

    public static float WaitWrapper(int framesToWait)
    {
        return Timing.WaitUntilDone(Timing.RunCoroutine(Wait(framesToWait)));
    }
}