using UnityEngine;

public abstract class MathUtil
{
    public static Vector2 GetUnitVectorPointingAt(float angleDegrees) {
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angleDegrees), Mathf.Sin(Mathf.Deg2Rad * angleDegrees));
    }
}