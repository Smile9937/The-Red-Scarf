using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundEffect : MonoBehaviour
{
    [EventRef]
    public string Event = "";

    [SerializeField] private float intensity;
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);

            float volume = Mathf.Clamp(1 - (0.1f * (distance / intensity)), 0, 1);

            AudioLibrary.Instance.ChangeClipVolume(Event, volume);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioLibrary.Instance.ChangeClipVolume(Event, 0);
        }
    }
}
