using System.Collections.Generic;
using MEC;

public class CoroutineUtil
{
    private const string TAG_STRING_SINGLE_LOOP = "singleLoop";
    private const string TAG_STRING_BULLET_SPAWNING = "bulletSpawn";

    public static CoroutineHandle StartSingleLoopCRT(IEnumerator<float> coroutine)
    {
        return Timing.RunCoroutine(coroutine, TAG_STRING_SINGLE_LOOP);
    }

    public static void KillSingleLoopCRT()
    {
        Timing.KillCoroutines(TAG_STRING_SINGLE_LOOP);
    }

    public static CoroutineHandle StartBulletSpawningCRT(IEnumerator<float> coroutine)
    {
        return Timing.RunCoroutine(coroutine, TAG_STRING_BULLET_SPAWNING);
    }

    public static void KillBulletSpawningCRT()
    {
        Timing.KillCoroutines(TAG_STRING_BULLET_SPAWNING);
    }
}