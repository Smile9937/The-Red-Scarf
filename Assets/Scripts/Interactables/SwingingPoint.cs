using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPoint : ActivatableObject, IGrabbable
{
    public bool isSwingable = true;
    public float distanceBias = 0;
    [SerializeField] private GameObject theTarget;
    [SerializeField] CharacterGrapplingScarf theGrapplingScarf;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (FindObjectOfType<CharacterGrapplingScarf>() && theGrapplingScarf == null)
        {
            theGrapplingScarf = FindObjectOfType<CharacterGrapplingScarf>();
        }
    }

    protected override void Activate()
    {
        isSwingable = true;
    }

    protected override void DeActivate()
    {
        isSwingable = false;
    }

    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isSwingable)
        {
            if (theGrapplingScarf != null)
            {
                if (theGrapplingScarf.swingingPoint == null)
                {
                    theGrapplingScarf.SetSwingingPointAsTarget(theTarget, distanceBias);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (theGrapplingScarf != null)
            {
                theGrapplingScarf.SetSwingingPointAsTarget(null, 0);
            }
        }
    }
    */

    public void IsGrabbed()
    {
        if (theGrapplingScarf != null)
        {
            theGrapplingScarf.SetSwingingPointAsTarget(theTarget, distanceBias);
        }
    }
    public void HandleGrabbed()
    {
        if (theGrapplingScarf != null)
        {
            theGrapplingScarf.LaunchPlayerIntoDash();
            return;
        }
        ReturnFromGrabbed();
    }
    public void ReturnFromGrabbed()
    {
        theGrapplingScarf.ReturnPlayerState();
    }
}
