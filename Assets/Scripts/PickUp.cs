using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

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
            DoEvent();
        }
    }

    private void DoEvent()
    {
        switch (type)
        {
            case Type.OneTimeUse:
                eventToStart.Invoke();
                Destroy(gameObject);
                break;
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
