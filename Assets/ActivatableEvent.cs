using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableEvent : ActivatableObject
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

    protected override void Activate()
    {
        switch (type)
        {
            case Type.OneTimeUse:
                eventToStart.Invoke();
                Destroy(this);
                break;
            case Type.MultiUse:
                eventToStart.Invoke();
                break;
        }
    }

    protected override void DeActivate()
    {
        switch(type)
        {
            case Type.MultiUse:
                eventToStart.Invoke();
                break;
            case Type.Toggle:
                if (toggle)
                {
                    eventToStart.Invoke();
                }
                else
                {
                    toggleEvent.Invoke();
                }
                toggle = !toggle;
                break;
        }
    }
}
