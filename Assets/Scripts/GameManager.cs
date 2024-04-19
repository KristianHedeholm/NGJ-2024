using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Hit stop time
    private static float waitTime = 0.0f;

    private static bool paused;
    public static bool Paused
    {
        get { return paused; }
        set
        {
            paused = value;
            if (paused)
                OnGamePaused?.Invoke();
            else
                OnGameUnPaused?.Invoke();
        }
    }
    public static float TimeSinceLastPause;

    public static Action OnGamePaused;
    public static Action OnGameUnPaused;

    // Uncomment this and change the type name to your player class for a
    // globally accessible player reference.
    /*
    private PlayerController playerRef;
    public static PlayerController PlayerRef
    {
        get
        {
            Instance.playerRef ??= FindObjectOfType<PlayerController>();

            return Instance.playerRef;
        }
    }
    */

    private void Update()
    {
#if DEBUG
        // Quit debug builds of the game by pressing escape.
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        // Slow-mo debug key.
        if (Input.GetKeyDown(KeyCode.F))
            Globals.TimeScale = Globals.TimeScale == 0.3f ? 1.0f : 0.3f;
#endif
        TimeSinceLastPause += Time.unscaledDeltaTime;

        // Stop time while paused
        if (Paused)
        {
            Time.timeScale = 0f;
            TimeSinceLastPause = 0f;
        }
        // Hitstop logic
        else if (waitTime > 0f)
        {
            waitTime -= Time.unscaledDeltaTime;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = Globals.TimeScale;
        }
    }

    /// <summary>
    /// Pauses the game for a specified amount of time.
    /// Useful for hitstop effects when passing small values like 0.1f
    /// </summary>
    /// <param name="time">How long to pause for</param>
    /// <param name="additive">If true, will add to waitTime instead of setting it</param>
    public static void WaitFor(float time, bool additive = false)
    {
        if (additive)
            waitTime += time;
        else
            waitTime = time;
    }
}
