using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioLibrary : MonoBehaviour
{
    private EventInstance playMusic;

    public List<EventInstance> sounds = new List<EventInstance>();

    public SongEnum songEnum;
    public enum SongEnum
    {
        Demo_2_2,
        Demo_2_1,
        Demo_3,
        TRS_LOOP_123_REV2
    }

    private static AudioLibrary instance;
    public static AudioLibrary Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }

        playMusic = RuntimeManager.CreateInstance("event:/Music/Background Music");
        switch(songEnum)
        {
            case SongEnum.Demo_2_2:
                PlayMusic(1);
                break;
            case SongEnum.Demo_2_1:
                PlayMusic(0);
                break;
            case SongEnum.Demo_3:
                PlayMusic(2);
                break;
            case SongEnum.TRS_LOOP_123_REV2:
                PlayMusic(3);
                break;
        }
    }
    bool IsPlaying(EventInstance instance)
    {
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
    }

    public string GetInstantiatedEventName(EventInstance instance)
    {
        string result;
        EventDescription description;

        instance.getDescription(out description);
        description.getPath(out result);

        // expect the result in the form event:/folder/sub-folder/eventName
        return result;

    }

    public void PlayMusic(int musicTrack)
    {
        if (!IsPlaying(playMusic))
        {
            playMusic.start();
            playMusic.setParameterByName("Current Music", musicTrack);
        }
    }

    public void StopMusic()
    {
        playMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ChangeMusicParameter(int musicTrack)
    {
        PlayMusic(musicTrack);
        playMusic.setParameterByName("Current Music", musicTrack);
    }

    public bool SoundExists(string Event)
    {
        foreach(EventInstance currentSound in sounds)
        {
            if (GetInstantiatedEventName(currentSound) == Event)
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeClipVolume(string Event, float volume)
    {
        if (!SoundExists(Event))
            return;

        foreach(EventInstance currentSound in sounds)
        {
            if(GetInstantiatedEventName(currentSound) == Event)
            {
                currentSound.setVolume(volume);
                return;
            }
        }
    }
}