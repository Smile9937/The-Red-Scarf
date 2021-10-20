using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
