using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableObject : MonoBehaviour
{
    [SerializeField] protected int id;
    protected virtual void Start()
    {
        GameEvents.instance.onActivate += Activate;
        GameEvents.instance.onDeActivate += DeActivate;
    }

    private void Activate(int id)
    {
        if(this.id == id)
        {
            Activate();
        }
    }
    private void DeActivate(int id)
    {
        if(this.id == id)
        {
            DeActivate();
        }
    }
    protected virtual void OnDestroy()
    {
        GameEvents.instance.onActivate -= Activate;
        GameEvents.instance.onDeActivate -= DeActivate;
    }
    protected abstract void Activate();
    protected abstract void DeActivate();
}
