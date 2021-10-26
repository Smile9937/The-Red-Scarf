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
    [SerializeField] float dashXStrength = 1.75f;
    [SerializeField] float dashYStrength = 3f;
    [SerializeField] float dashDelayBeforeStart = 0.2f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float gravityAdjustment = 0.5f;

    float characterGravity = 1f;

    Vector2 originalLaunchPosition;
    Vector2 targetLaunchPosition;
    
    Player player;
    Animator animator;

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        characterGravity = characterRigidBody.gravityScale;
        isSwinging = false;
    }
    void Update()
    {
        if (swingingPoint != null)
        {
            if (Input.GetKeyDown(KeyCode.F) && !isSwinging)
            {
                ToggleIsSwinging();
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasStartedSwing && isSwinging && swingingPoint != null)
        {
            characterRigidBody.velocity = new Vector2((originalLaunchPosition.x - targetLaunchPosition.x) * Mathf.Abs(dashXStrength), (originalLaunchPosition.y - targetLaunchPosition.y) * Mathf.Abs(dashYStrength)) * -1f;
            if (Mathf.Approximately(Mathf.Abs(originalLaunchPosition.x - targetLaunchPosition.x) + Mathf.Abs(originalLaunchPosition.y - targetLaunchPosition.y), 0.1f))
            {
                ReturnPlayerState();
                isSwinging = false;
            }
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
        hasStartedSwing = false;
        isSwinging = !isSwinging;
        if (swingingPoint != null)
        {
            swingingPoint.GetComponentInParent<SwingingPoint>().isSwingingFrom = isSwinging;
        }
        if (isSwinging)
        {
            animator.SetBool("isScarfThrown", true);
            if (CheckPrimaryTarget() && swingingPoint != null)
            {
                targetLaunchPosition = swingingPoint.transform.position;
                characterRigidBody.gravityScale = gravityAdjustment;
            }
            else if (swingingPointAlt != null)
            {
                targetLaunchPosition = swingingPointAlt.transform.position;
                characterRigidBody.gravityScale = gravityAdjustment;
            }
            CancelInvoke("ToggleIsSwinging");
            CancelInvoke("ReturnPlayerState");
            CancelInvoke("DelayBeforeSwingStart");
            Invoke("ToggleIsSwinging", dashDuration + dashDelayBeforeStart);
            Invoke("ReturnPlayerState", dashDuration + dashDelayBeforeStart + 0.1f);
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
        characterRigidBody.gravityScale = characterGravity;
        swingingPoint = null;
        swingingPointAlt = null;
        targetLaunchPosition = new Vector2(0,0);
    }

    private void DelayBeforeSwingStart()
    {
        Debug.Log("S");
        hasStartedSwing = true;
        animator.SetBool("isScarfThrown", false);
    }
}
