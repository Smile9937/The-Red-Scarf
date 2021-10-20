using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPoint : ActivatableObject
{
    public bool isSwingingFrom = false;
    public bool isSwingable = true;
    public DistanceJoint2D theSwingJoint;

    // Start is called before the first frame update
    void Start()
    {
        if (theSwingJoint == null)
        {
            theSwingJoint = GetComponent<DistanceJoint2D>();
        }
        theSwingJoint.enabled = false;
    }

    public override void Activate()
    {
        isSwingable = true;
    }

    public override void Deactivate()
    {
        isSwingable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isSwingable)
        {
            if (collision.GetComponent<CharacterGrapplingScarf>() && !isSwingingFrom)
            {
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(this.gameObject);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<CharacterGrapplingScarf>() && !isSwingingFrom)
            {
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(null);
            }
        }
    }
}
