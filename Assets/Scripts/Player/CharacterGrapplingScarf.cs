using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    [Header("Components")]
    bool isSwinging = false;
    bool hasStartedSwing = false;
    [SerializeField] GameObject swingingPoint;
    [SerializeField] GameObject swingingPointAlt;
    [SerializeField] Rigidbody2D characterRigidBody;
    [Header("Stats")]
    [SerializeField] float dashStrength = 4f;
    [SerializeField] float dashDelayBeforeStart = 0.2f;
    [SerializeField] float dashDuration = 1f;

    Vector2 originalLaunchPosition;
    Vector2 targetLaunchPosition;
    
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
            characterRigidBody.velocity = new Vector2(originalLaunchPosition.x - targetLaunchPosition.x, originalLaunchPosition.y - targetLaunchPosition.y) * Mathf.Abs(dashStrength) * -1f;
        }
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint, GameObject swingPointB)
    {
        swingingPoint = swingPoint;
        swingingPointAlt = swingPointB;
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
            swingingPoint.GetComponentInParent<SwingingPoint>().isSwingingFrom = isSwinging;
        }
        if (isSwinging)
        {
            if (CheckPrimaryTarget() && swingingPoint != null)
            {
                targetLaunchPosition = swingingPoint.transform.position;
            }
            else if (swingingPointAlt != null)
            {
                targetLaunchPosition = swingingPointAlt.transform.position;
            }
            CancelInvoke("ToggleIsSwinging");
            CancelInvoke("ReturnPlayerState");
            Invoke("ToggleIsSwinging", dashDuration + dashDelayBeforeStart);
            Invoke("ReturnPlayerState", dashDuration + dashDelayBeforeStart + 0.1f);
            CancelInvoke("DelayBeforeSwingStart");
            Invoke("DelayBeforeSwingStart", dashDelayBeforeStart);
        }
    }

    private bool CheckPrimaryTarget()
    {
        if (swingingPoint != null && swingingPointAlt == null)
        {
            return true;
        }
        else if (swingingPoint != null && swingingPointAlt != null)
        {
            if (Mathf.Abs(transform.position.x - swingingPoint.transform.position.x) + Mathf.Abs(transform.position.y - swingingPoint.transform.position.y) >= Mathf.Abs(transform.position.x - swingingPointAlt.transform.position.x) + Mathf.Abs(transform.position.y - swingingPointAlt.transform.position.y))
            {
                return true;
            }
        }
        return false;
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
