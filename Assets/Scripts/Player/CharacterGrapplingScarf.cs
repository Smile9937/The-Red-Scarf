using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    bool isSwinging = false;
    [SerializeField] GameObject swingingPoint;
    [SerializeField] Rigidbody2D characterRigidBody;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || (Input.GetButtonDown("Jump") && isSwinging))
        {
            ToggleIsSwinging();
        }
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint)
    {
        swingingPoint = swingPoint;
    }

    private void ToggleIsSwinging()
    {
        isSwinging = !isSwinging;
        if (swingingPoint != null)
        {
            Debug.Log(isSwinging);
            swingingPoint.GetComponent<SwingingPoint>().isSwingingFrom = isSwinging;
            swingingPoint.GetComponent<DistanceJoint2D>().enabled = isSwinging;
        }
    }
}
