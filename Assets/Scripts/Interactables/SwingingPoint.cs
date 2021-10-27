using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPoint : ActivatableObject
{
    public bool isSwingingFrom = false;
    public bool isSwingable = true;
    public DistanceJoint2D theSwingJoint;
    [SerializeField] private GameObject theFirstTarget;
    [SerializeField] private GameObject theSecondTarget;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (theSwingJoint == null)
        {
            theSwingJoint = GetComponent<DistanceJoint2D>();
        }
        theSwingJoint.enabled = false;
    }

    protected override void Activate()
    {
        isSwingable = true;
    }

    protected override void DeActivate()
    {
        isSwingable = false;
        theSwingJoint.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isSwingable)
        {
            if (collision.GetComponent<CharacterGrapplingScarf>())
            {
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(theFirstTarget, theSecondTarget);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<CharacterGrapplingScarf>() && !isSwingingFrom)
            {
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(null, null);
            }
        }
    }
}
