using UnityEngine;

public static class NumberExtensions
{
    /// <summary>
    /// Maps one range to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Calculate a vector from a given radian angle.
    /// </summary>
    /// <param name="radians"></param>
    /// <param name="length">Length of the resulting vector</param>
    /// <returns></returns>
    public static Vector2 AngleToDir(this float radians, float length = 1)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * length;
    }

    /// <summary>
    /// Calculate a vector from a given angle in degrees.
    /// </summary>
    /// <param name="degrees"></param>
    /// <param name="length">Length of the resulting vector</param>
    /// <returns></returns>
    public static Vector2 DegAngleToDir(this float degrees, float length = 1)
    {
        return (degrees * Mathf.Deg2Rad).AngleToDir(length);
    }
}
