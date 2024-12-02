using UnityEngine;

public static class ROFScaling
{
    public const int MIN_ROF = 0;
    public const int MAX_ROF = 1500;

    private const int MIN_FRAMES = 5;
    private const int MAX_FRAMES = 60;

    public static int GetFramesBetweenShot(int rateOfFire, int rof1 = MIN_ROF, int frames1 = MAX_FRAMES, int rof2 = MAX_ROF, int frames2 = MIN_FRAMES)
    {
        int b = Mathf.CeilToInt((float)(frames2 * rof2 - frames1 * rof1) / (frames1 - frames2));
        int a = frames1 * (b + rof1);
        rateOfFire = rateOfFire > MAX_ROF ? MAX_ROF : rateOfFire;
        int frames = (int)((float)a / (b + rateOfFire));
        return frames < 1 ? 1 : frames;
    }
}