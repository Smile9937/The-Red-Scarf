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
    
    public void IsGrabbed()
    {
        if (theGrapplingScarf != null && isSwingable)
        {
            theGrapplingScarf.SetSwingingPointAsTarget(theTarget, distanceBias);
        }
    }
    public void HandleGrabbedTowards()
    {
        if (theGrapplingScarf != null)
        {
            theGrapplingScarf.LaunchPlayerIntoDash();
            return;
        }
    }
    public void HandleGrabbedAway()
    {
        Invoke("ReturnFromGrabbed", 0.1f);
    }
    public void ReturnFromGrabbed()
    {
        theGrapplingScarf.ReturnPlayerState();
    }
}
