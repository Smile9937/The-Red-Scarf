using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidAnimationPlayer : MonoBehaviour
{
    private Animator myAnimator;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    private void DisableGameobject()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        myAnimator.SetTrigger("drop");
    }
}
