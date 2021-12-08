using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [EventRef]
    public string[] Events;

    private List<EventInstance> playSounds = new List<EventInstance>();
    void Awake()
    {
        foreach(string Event in Events)
        {
            playSounds.Add(RuntimeManager.CreateInstance(Event));
        }
    }

    public void PlaySound(int id)
    {
        playSounds[id].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        playSounds[id].start();
    }

    public void StopSound(int id)
    {
        playSounds[id].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ChangeSoundParameter(int id, string soundParameter, int value)
    {
        playSounds[id].setParameterByName(soundParameter, value);
    }

    private void OnDestroy()
    {
        foreach(EventInstance sound in playSounds)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
        }
    }
}