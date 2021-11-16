using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour
{
    public UnityEvent eventToStart;

    public UnityEvent toggleEvent;

    private bool toggle = true;

    public Type type;
    public enum Type
    {
        OneTimeUse,
        MultiUse,
        Toggle
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            switch(type)
            {
                case Type.OneTimeUse:
                    eventToStart.Invoke();
                    DoNotRespawn doNotRespawn = GetComponent<DoNotRespawn>();
                    if(doNotRespawn != null)
                    {
                        doNotRespawn.Collected();
                    }
                    Destroy(gameObject);
                    break;
                case Type.MultiUse:
                    eventToStart.Invoke();
                    break;
                case Type.Toggle:
                    toggle = !toggle;
                    if(toggle)
                    {
                        eventToStart.Invoke();
                    }
                    else
                    {
                        toggleEvent.Invoke();
                    }
                    break;
            }
        }
    }
}
