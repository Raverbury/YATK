using UnityEngine;

public abstract class MathUtil
{
    public static Vector2 GetUnitVectorPointingAt(float angleDegrees) {
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angleDegrees), Mathf.Sin(Mathf.Deg2Rad * angleDegrees));
    }

    public static int Sum(int from, int to) {
        if (to < from) {
            return 0;
        }
        int res = 0;
        for (int i = from; i <= to; i++) {
            res += i;
        }
        return res;
    }
}