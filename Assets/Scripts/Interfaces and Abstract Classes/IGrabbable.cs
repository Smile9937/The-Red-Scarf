using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    
    void IsGrabbed();

    void ReturnFromGrabbed();

    void HandleGrabbedTowards();
    void HandleGrabbedAway();
}
