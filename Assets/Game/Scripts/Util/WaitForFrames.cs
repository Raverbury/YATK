using System.Collections.Generic;
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
}