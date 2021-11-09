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

    private void Start()
    {
        animator = GetComponent<Animator>();
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
    
    private void Activate()
    {
        GameEvents.instance.Activate(id);
    }
    private void Deactivate()
    {
        GameEvents.instance.DeActivate(id);
    }
}
