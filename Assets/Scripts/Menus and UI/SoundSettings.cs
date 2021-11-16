using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundSettings : MonoBehaviour
{
    private Bus music;
    private Bus SFX;
    private Bus master;

    float musicVolume = 0.5f;
    float SFXVolume = 0.5f;
    float masterVolume = 0.5f;
    private void Awake()
    {
        music = RuntimeManager.GetBus("bus:/Master/Music");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        master = RuntimeManager.GetBus("bus:/Master");
    }

    void Update()
    {
        music.setVolume(musicVolume);
        SFX.setVolume(SFXVolume);
        master.setVolume(masterVolume);
    }

    public void ChangeMasterVolume(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
    }
    public void ChangeMusicVolume(float newMusicVolume)
    {
        musicVolume = newMusicVolume;
    }
    public void ChangeSFXVolume(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;
    }
}