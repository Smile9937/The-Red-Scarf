using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPoint : ActivatableObject, IGrabbable
{
    public bool isSwingable = true;
    public float distanceBias = 0;
    [SerializeField] private GameObject theTarget;
    [SerializeField] CharacterGrapplingScarf theGrapplingScarf;
    [SerializeField] SpriteRenderer theSpriteRenderer = null;
    [SerializeField] Sprite unGrabbedSprite;
    [SerializeField] Sprite grabbedSprite;
    bool hasSpriteToUse = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (FindObjectOfType<CharacterGrapplingScarf>() && theGrapplingScarf == null)
        {
            theGrapplingScarf = FindObjectOfType<CharacterGrapplingScarf>();
        }
        if (theSpriteRenderer != null && unGrabbedSprite != null && grabbedSprite != null)
        {
            hasSpriteToUse = true;
            theSpriteRenderer.sprite = unGrabbedSprite;
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
            if (hasSpriteToUse)
                theSpriteRenderer.sprite = grabbedSprite;
        }
    }
    public void HandleGrabbedTowards()
    {
        if (theGrapplingScarf != null)
        {
            theGrapplingScarf.LaunchPlayerIntoDash();
            Invoke("ReturnSwingingPointSprite",0.1f);
            return;
        }
    }
    public void HandleGrabbedAway()
    {
        Invoke("ReturnFromGrabbed", 0.1f);
        Invoke("ReturnSwingingPointSprite", 0.1f);
    }
    public void ReturnFromGrabbed()
    {
        theGrapplingScarf.ReturnPlayerState();
    }

    public void ReturnSwingingPointSprite()
    {
        if (hasSpriteToUse)
            theSpriteRenderer.sprite = unGrabbedSprite;
    }
}
