using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour, IDamageable
{
    [Tooltip("Only Activatable Objects")]
    public int id;

    public Type type;
    public enum Type
    {
        Button,
        Lever,
        OneTimeUseLever,
    }
    public float timer;

    public bool active = false;

    private bool canBeUsed = true;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        GameEvents.Instance.onSaveGame += Save;
        GameEvents.Instance.onLoadGame += Load;
    }
    private void OnDisable()
    {
        GameEvents.Instance.onSaveGame -= Save;
        GameEvents.Instance.onLoadGame -= Load;
    }

    public void Damage(int damage, bool bypassInvincibility)
    {
        switch(type)
        {
            case Type.Button:
                {
                    Activate();
                    animator.SetBool("isActivated", true);
                    if (timer > 0)
                    {
                        StartCoroutine(Timer());
                    }
                }
                break;
            case Type.Lever:
                active = !active;
                animator.SetBool("isActivated", active);
                if (active)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
                break;
            case Type.OneTimeUseLever:
                if (!canBeUsed) return;
                active = true;
                canBeUsed = false;
                animator.SetBool("isActivated", true);
                Activate();
                break;
        }
    }
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timer);

        animator.SetBool("isActivated", false);
        Deactivate();
    }

    public void ActivateFromOtherObject()
    {
        Activate();
    }
    
    private void Activate()
    {
        GameEvents.Instance.Activate(id);
    }
    private void Deactivate()
    {
        GameEvents.Instance.DeActivate(id);
    }

    private void Save()
    {
        if (active)
        {
            if (GameManager.Instance.activatorLocations.Contains(transform.position))
                return;

            GameManager.Instance.activatorLocations.Add(transform.position);
        }
        else
        {
            if (!GameManager.Instance.activatorLocations.Contains(transform.position))
                return;

            GameManager.Instance.activatorLocations.Remove(transform.position);
        }
    }
    private void Load()
    {
        foreach(Vector3 position in GameManager.Instance.activatorLocations)
        {
            if(position == transform.position)
            {
                Damage(0, false);
            }
        }
    }
}
