using System.Collections.Generic;
using MEC;
using Unity.Entities;

public class CoroutineUtil
{
    private const string TAG_STRING_SINGLE_LOOP = "singleLoop";
    public const string TAG_STRING_PLAYER_INVULNERABLE = "playerInvulnerable";

    /// <summary>
    /// Starts an MEC coroutine that is cleared automatically after a single concludes.<br/>
    /// Best used for coroutines you don't want to persist between singles.
    /// </summary>
    /// <param name="coroutine"></param>
    /// <returns></returns>
    public static CoroutineHandle StartSingleLoopCRT(IEnumerator<float> coroutine)
    {
        return Timing.RunCoroutine(coroutine, TAG_STRING_SINGLE_LOOP);
    }

    /// <summary>
    /// Kills all MEC coroutines ran with StartSingleLoopCRT.
    /// </summary>
    public static void KillSingleLoopCRT()
    {
        Timing.KillCoroutines(TAG_STRING_SINGLE_LOOP);
    }

    public static void RunPlayerInvulnerableCoroutine(IEnumerator<float> coroutine)
    {
        Timing.KillCoroutines(TAG_STRING_PLAYER_INVULNERABLE);
        Timing.RunCoroutine(coroutine, TAG_STRING_PLAYER_INVULNERABLE);
    }

    /// <summary>
    /// Starts an MEC coroutine and binds it to an Entity. When that Entity despawns (expected through ECSEntitySpawner), the bound coroutine is destroyed.
    /// </summary>
    /// <param name="coroutine"></param>
    /// <param name="entity"></param>
    public static void RunEntityBoundCoroutine(IEnumerator<float> coroutine, Entity entity)
    {
        Timing.RunCoroutine(coroutine, $"Entity#{entity.Index}");
    }

    /// <summary>
    /// Destroyes any MEC coroutines bound to an Entity.
    /// </summary>
    /// <param name="entity"></param>
    public static void KillEntityBoundCoroutines(Entity entity)
    {
        Timing.KillCoroutines($"Entity#{entity.Index}");
    }
}