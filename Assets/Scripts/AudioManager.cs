using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private EventInstance ambInstance;

    private List<EventInstance> eventInstances = new();

    private List<EventInstance> instancesToRemove = new();

    private void FixedUpdate()
    {
        if (instancesToRemove.Count == 0)
            return;

        bool clearList = true;
        foreach (var inst in instancesToRemove)
        {
            if (!inst.isValid())
                continue;
            if (inst.getPlaybackState(out var state) == FMOD.RESULT.OK)
            {
                switch (state)
                {
                    case PLAYBACK_STATE.SUSTAINING:
                    case PLAYBACK_STATE.STARTING:
                    case PLAYBACK_STATE.PLAYING:
                        inst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                        break;
                    case PLAYBACK_STATE.STOPPING: // Allow fade out
                        clearList = false;
                        continue;
                    case PLAYBACK_STATE.STOPPED:
                        if (eventInstances.Contains(inst))
                            eventInstances.Remove(inst);
                        inst.release();
                        break;
                }
            }
        }

        if (clearList)
            instancesToRemove.Clear();
    }

    public void StartAmbience(EventReference ambience)
    {
        ambInstance = CreateInstance(ambience);
        ambInstance.start();
    }

    public void SetGlobalParameter(string parameterName, float parameterValue)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    }

    public static void PlayOneShot(EventReference sound, Vector3 worldPos = default)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void RemoveInstance(EventInstance instance)
    {
        instancesToRemove.Add(instance);
    }

    private void CleanUp()
    {
        foreach (var instance in eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
