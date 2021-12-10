using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundEffect : MonoBehaviour
{
    [EventRef]
    public string Event = "";

    [SerializeField] private SoundPlayer soundPlayer;

    [SerializeField] private int soundId;

    [SerializeField] private float intensity;

    //Backup for Respawn
    private void Start()
    {
        soundPlayer.ChangeVolume(soundId, 0, this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);

            float volume = Mathf.Clamp(1 - (0.1f * (distance / intensity)), 0, 1);

            soundPlayer.ChangeVolume(soundId, volume, this);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            soundPlayer.ChangeVolume(soundId, 0, this);
        }
    }
}
