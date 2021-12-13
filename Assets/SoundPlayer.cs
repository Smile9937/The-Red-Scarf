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
    private float highestVolume;
    private SoundEffect currentSoundEffect;
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

    public void ChangeVolume(int id, float volume, SoundEffect soundEffect)
    {
        if(soundEffect == currentSoundEffect)
        {
            highestVolume = volume;
        }
        if(soundEffect != currentSoundEffect)
        {
            if(volume > highestVolume)
            {
                highestVolume = volume;
                currentSoundEffect = soundEffect;
            }
        }

        playSounds[id].setVolume(highestVolume);
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