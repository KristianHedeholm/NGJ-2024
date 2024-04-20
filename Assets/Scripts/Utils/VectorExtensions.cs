using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Rotate a vector by the given degrees.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static Vector2 RotateDeg(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /// <summary>
    /// Rotate the vector by the given angle in radians.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static Vector2 Rotate(this Vector2 v, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /// <summary>
    /// Calculate the angle of the given vector.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>The angle in degrees</returns>
    public static float ToAngleDeg(this Vector2 v)
    {
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Calculate the angle of the given vector.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>The angle in radians</returns>
    public static float ToAngle(this Vector2 v)
    {
        return Mathf.Atan2(v.x, v.y);
    }
}
