using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleBossIntroAnimation : MonoBehaviour
{
    private const string START = "Start";

    TripleBossManager bossManager;
    Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        bossManager = GetComponentInParent<TripleBossManager>();
    }
    public void StartAnimation()
    {
        myAnimator.Play(START);
    }

    //Called In Animator
    private void IntroFinished()
    {
        bossManager.PrepareForBattle();

        TripleBoss[] tripleBosses = GetComponentsInChildren<TripleBoss>();

        foreach(TripleBoss boss in tripleBosses)
        {
            boss.transform.parent = bossManager.transform;
        }

        myAnimator.enabled = false;
    }
}