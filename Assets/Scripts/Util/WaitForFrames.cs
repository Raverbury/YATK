using UnityEngine;

class WaitForFrames : CustomYieldInstruction
{
    private int framesElapsed = 0;
    private int framesToWait;

    public override bool keepWaiting
    {
        get
        {
            return framesElapsed++ < framesToWait;
        }
    }

    public WaitForFrames(int framesToWait)
    {
        framesElapsed = 0;
        this.framesToWait = framesToWait;
    }
}