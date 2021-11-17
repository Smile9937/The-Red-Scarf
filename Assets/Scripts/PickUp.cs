using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour
{
    public UnityEvent eventToStart;

    public UnityEvent toggleEvent;

    private bool toggle = true;

    Collider2D myCollider;
    SpriteRenderer mySpriteRenderer;

    public Type type;
    public enum Type
    {
        OneTimeUse,
        MultiUse,
        Toggle
    }
    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
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
                DoNotRespawn doNotRespawn = GetComponent<DoNotRespawn>();
                if (doNotRespawn != null)
                {
                    doNotRespawn.Collected();
                }
                if (myCollider != null)
                    myCollider.enabled = false;
                if (mySpriteRenderer != null)
                    mySpriteRenderer.enabled = false;
                enabled = false;
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
