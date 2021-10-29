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
    [SerializeField] private GameObject[] thePossibleTargets;

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
                if (thePossibleTargets.Length > 0)
                {
                    int theTargetNum = 0;
                    foreach (var target in thePossibleTargets)
                    {
                        if (target != null)
                        {
                            collision.GetComponent<CharacterGrapplingScarf>().SetNewSwingingPointsAsTarget(target, theTargetNum);
                        }
                        theTargetNum++;
                    }
                }
                else
                {
                    collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(theFirstTarget, theSecondTarget);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<CharacterGrapplingScarf>())
            {
                int theTargetNum = thePossibleTargets.Length;
                foreach (var target in thePossibleTargets)
                {
                    theTargetNum--;
                    collision.GetComponent<CharacterGrapplingScarf>().SetNewSwingingPointsAsTarget(null, theTargetNum);
                }
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(null, null);
            }
        }
    }
}
