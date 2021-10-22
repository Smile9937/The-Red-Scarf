using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivatableObject
{
    protected override void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    protected override void DeActivate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
