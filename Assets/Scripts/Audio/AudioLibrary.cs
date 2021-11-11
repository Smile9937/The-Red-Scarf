using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioLibrary : MonoBehaviour
{
    FMOD.Studio.EventInstance playMusic;

    private static AudioLibrary instance;
    public static AudioLibrary Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        playMusic = RuntimeManager.CreateInstance("event:/Background Music");
        PlayMusic(0);
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
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
}