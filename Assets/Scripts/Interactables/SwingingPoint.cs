using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPoint : ActivatableObject
{
    public bool isSwingingFrom = false;
    public bool isSwingable = true;
    [SerializeField] private GameObject theTarget;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Activate()
    {
        isSwingable = true;
    }

    protected override void DeActivate()
    {
        isSwingable = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isSwingable)
        {
            if (collision.GetComponent<CharacterGrapplingScarf>())
            {
                if (collision.GetComponent<CharacterGrapplingScarf>().swingingPoint == null)
                {
                    collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(theTarget);
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
                collision.GetComponent<CharacterGrapplingScarf>().SetSwingingPointAsTarget(null);
            }
        }
    }
}
