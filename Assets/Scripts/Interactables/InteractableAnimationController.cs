using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAnimationController : ActivatableObject
{
    [SerializeField] bool isStoppable;
    Animator animator = null;

    // Mainly made for incredibly simple animations that trigger when a switch is activated

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Activate()
    {
        animator.SetTrigger("startAnim");
    }

    protected override void DeActivate()
    {
        if (isStoppable)
        {
            animator.SetTrigger("stopAnim");
        }
    }
}
