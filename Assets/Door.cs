using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivatableObject
{
    public override void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void Deactivate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
