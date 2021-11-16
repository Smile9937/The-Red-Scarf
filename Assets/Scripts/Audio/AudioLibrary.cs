using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioLibrary : MonoBehaviour
{
    FMOD.Studio.EventInstance playMusic;

    List<FMOD.Studio.EventInstance> sounds = new List<FMOD.Studio.EventInstance>();

    public SongEnum songEnum;
    public enum SongEnum
    {
        Demo_2_2,
        Demo_2_1,
        Demo_3
    }

    private static AudioLibrary instance;
    public static AudioLibrary Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
    }
    private void Start()
    {
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
        }
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    public string GetInstantiatedEventName(FMOD.Studio.EventInstance instance)
    {
        string result;
        FMOD.Studio.EventDescription description;

        instance.getDescription(out description);
        description.getPath(out result);

        // expect the result in the form event:/folder/sub-folder/eventName
        return result;

    }

    private void PlayMusic(int musicTrack)
    {
        if (!IsPlaying(playMusic))
        {
            playMusic.start();
            playMusic.setParameterByName("Current Music", musicTrack);
        }
    }

    public void ChangeMusicParameter(int musicTrack)
    {
        playMusic.setParameterByName("Current Music", musicTrack);
    }

    public void PlaySound(string Event)
    {
        FMOD.Studio.EventInstance currentSound = RuntimeManager.CreateInstance(Event);
        sounds.Add(currentSound);
        currentSound.start();
        Debug.Log(GetInstantiatedEventName(currentSound));
    }

    public void ChangeClipVolume(string Event, float volume) {

        foreach(FMOD.Studio.EventInstance currentSound in sounds)
        {
            if(GetInstantiatedEventName(currentSound) == Event)
            {
                currentSound.setVolume(volume);
            }
        }
    }
}