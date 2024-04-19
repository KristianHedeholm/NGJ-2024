
/// <summary>
/// Some useful constants
/// </summary>
public static class Globals
{
    /// <summary>
    /// One sixtieth of a second. Useful when thinking of metrics of fixed frames, while keeping the game's actual framerate unlocked.
    /// </summary>
    public const float OneFrame = 1.0f / 60.0f;
    /// <summary>
    /// TimeScale - Use this instead of Time.timeScale, since that's overriden in the GameManager class.
    /// </summary>
    public static float TimeScale = 1.0f;
}