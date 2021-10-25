using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    bool isSwinging = false;
    bool hasStartedSwing = false;
    [SerializeField] GameObject swingingPoint;
    [SerializeField] Rigidbody2D characterRigidBody;
    [SerializeField] float dashStrength = 4f;
    [SerializeField] float dashDelayBeforeStart = 0.2f;
    [SerializeField] float dashDuration = 1f;

    Vector2 originalLaunchPosition;

    Player player;
    private void Start()
    {
        player = GetComponent<Player>();
        isSwinging = false;
    }
    void Update()
    {
        if (swingingPoint != null)
        {
            if (Input.GetKeyDown(KeyCode.F) || (Input.GetButtonDown("Jump") && isSwinging))
            {
                ToggleIsSwinging();
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasStartedSwing && isSwinging && swingingPoint != null)
        {
            characterRigidBody.velocity = new Vector2(originalLaunchPosition.x - swingingPoint.transform.position.x, originalLaunchPosition.y - swingingPoint.transform.position.y) * Mathf.Abs(dashStrength) * -1f;
        }
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint)
    {
        swingingPoint = swingPoint;
    }

    private void ToggleIsSwinging()
    {
        if (!isSwinging)
        {
            player.state = Player.State.Dash;
            originalLaunchPosition = transform.position;
        }
        isSwinging = !isSwinging;
        if (swingingPoint != null)
        {
            Debug.Log(isSwinging);
            swingingPoint.GetComponent<SwingingPoint>().isSwingingFrom = isSwinging;
            //swingingPoint.GetComponent<DistanceJoint2D>().enabled = isSwinging;
        }
        if (isSwinging)
        {
            CancelInvoke("ToggleIsSwinging");
            CancelInvoke("ReturnPlayerState");
            Invoke("ToggleIsSwinging", dashDuration + dashDelayBeforeStart);
            Invoke("ReturnPlayerState", dashDuration + dashDelayBeforeStart + 0.1f);
            CancelInvoke("DelayBeforeSwingStart");
            Invoke("DelayBeforeSwingStart", dashDelayBeforeStart);
        }
    }

    private void ReturnPlayerState()
    {
        player.state = Player.State.Neutral;
    }

    private void DelayBeforeSwingStart()
    {
        hasStartedSwing = !hasStartedSwing;
    }
}
