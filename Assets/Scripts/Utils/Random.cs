using System;
using Mathf = UnityEngine.Mathf;
using Vector2 = UnityEngine.Vector2;
using Color = UnityEngine.Color;

namespace Utils
{
    /// <summary>
    /// Handy collection of functions for getting random numbers.
    /// </summary>
    public static class Random
    {
        private static int _seed = Environment.TickCount;

        private static System.Random rng = new(_seed);
        public static System.Random RNG { get { return rng; } }


        /// <summary>
        /// returns current seed value
        /// </summary>
        /// <returns>Seed.</returns>
        public static int GetSeed()
        {
            return _seed;
        }


        /// <summary>
        /// resets rng with new seed
        /// </summary>
        /// <param name="seed">Seed.</param>
        public static void SetSeed(int seed)
        {
            _seed = seed;
            rng = new System.Random(_seed);
        }


        /// <summary>
        /// returns a random float between 0 (inclusive) and 1 (exclusive)
        /// </summary>
        /// <returns>The float.</returns>
        public static float NextFloat()
        {
            return (float)rng.NextDouble();
        }


        /// <summary>
        /// returns a random float between 0 (inclusive) and max (exclusive)
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="max">Max.</param>
        public static float NextFloat(float max)
        {
            return (float)rng.NextDouble() * max;
        }


        /// <summary>
        /// returns a random int between 0 (inclusive) and max (exclusive)
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="max">Max.</param>
        public static int NextInt(int max)
        {
            return rng.Next(max);
        }


        /// <summary>
        /// returns a random float between 0 and 2 * PI
        /// </summary>
        /// <returns>Angle radians</returns>
        public static float NextAngle()
        {
            return (float)rng.NextDouble() * Mathf.PI * 2;
        }


        /// <summary>
        /// Returns a random unit vector with direction between 0 and 2 * PI
        /// </summary>
        public static Vector2 NextUnitVector()
        {
            float angle = NextAngle();
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }


        /// <summary>
        /// returns a random color
        /// </summary>
        public static Color NextColor()
        {
            return new Color(NextFloat(), NextFloat(), NextFloat());
        }


        /// <summary>
        /// Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Range(int min, int max)
        {
            return rng.Next(min, max);
        }


        /// <summary>
        /// Returns a random float between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Range(float min, float max)
        {
            return min + NextFloat(max - min);
        }


        /// <summary>
        /// Returns a random Vector2, and x- and y-values of which are between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static Vector2 Range(Vector2 min, Vector2 max)
        {
            return min + new Vector2(NextFloat(max.x - min.x), NextFloat(max.y - min.y));
        }


        /// <summary>
        /// returns a random float between -1 and 1
        /// </summary>
        public static float MinusOneToOne()
        {
            return NextFloat(2f) - 1f;
        }


        /// <summary>
        /// returns true if the next random is less than percent. Percent should be between 0 and 1
        /// </summary>
        /// <param name="percent">Percent</param>
        public static bool Chance(float percent)
        {
            return NextFloat() < percent;
        }


        /// <summary>
        /// returns true if the next random is less than value. Value should be between 0 and 100.
        /// </summary>
        /// <param name="value">Value</param>
        public static bool Chance(int value)
        {
            return NextInt(100) < value;
        }


        /// <summary>
        /// randomly returns one of the given values
        /// </summary>
        /// <param name="values">Values to choose from</param>
        /// <typeparam name="T">Type</typeparam>
        public static T Choose<T>(params T[] values)
        {
            int index = Range(0, values.Length);
            return values[index];
        }
    }
}

