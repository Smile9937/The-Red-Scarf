using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    [Header("Components")]
    bool isSwinging = false;
    bool hasStartedSwing = false;
    [SerializeField] public GameObject swingingPoint;
    [SerializeField] GameObject swingingPointAlt;
    [SerializeField] GameObject[] swingingPoints;
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
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Special) && GameManager.Instance.redScarf && player.state == Player.State.Neutral)
        {
            if (player.state != Player.State.Dash)
            {
                animator.SetTrigger("startScarfThrow");
                player.state = Player.State.Dash;
            }
            if (player.grounded)
            {
                characterRigidBody.velocity = Vector2.zero;
            }
            else
            {
                float temp = characterRigidBody.velocity.y;
                if (temp < -0.01f)
                {
                    temp *= -0.5f;
                }
                else if (temp > 0.01f)
                {
                    temp += 0.1f;
                }
                characterRigidBody.velocity = new Vector2(characterRigidBody.velocity.x * 0.6f, temp);
            }

            if ((swingingPoint != null || swingingPointAlt != null || swingingPoints[0] != null) && !isSwinging && GameManager.Instance.redScarf)
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

    public void SetNewSwingingPointsAsTarget(GameObject possibleTarget, int targetNumber)
    {
        swingingPoints[targetNumber] = possibleTarget;
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
            if (swingingPoints[0] != null)
            {
                SetTargets();
                originalLaunchPosition = swingingPoint.transform.position;
                targetLaunchPosition = swingingPointAlt.transform.position;
            }
            else if (CheckPrimaryTarget() && swingingPoint != null)
            {
                targetLaunchPosition = swingingPoint.transform.position;
                if (swingingPointAlt != null)
                {
                    originalLaunchPosition = swingingPointAlt.transform.position;
                }
            }
            else if (swingingPointAlt != null)
            {
                if (swingingPoint != null)
                {
                    originalLaunchPosition = swingingPoint.transform.position;
                }
                targetLaunchPosition = swingingPointAlt.transform.position;
            }
            characterRigidBody.gravityScale = gravityAdjustment;
            CancelInvoke("ToggleIsSwinging");
            CancelInvoke("ReturnPlayerState");
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

    private void SetTargets()
    {
        swingingPoint = swingingPoints[0];
        swingingPointAlt = swingingPoints[0];
        // Closest
        foreach (var target in swingingPoints)
        {
            if (target != null)
            {
                if (Mathf.Abs(transform.position.x - swingingPoint.transform.position.x) + Mathf.Abs(transform.position.y - swingingPoint.transform.position.y) > Mathf.Abs(transform.position.x - target.transform.position.x) + Mathf.Abs(transform.position.y - target.transform.position.y))
                {
                    swingingPoint = target;
                }
                if (Mathf.Abs(transform.position.x - swingingPointAlt.transform.position.x) + Mathf.Abs(transform.position.y - swingingPointAlt.transform.position.y) < Mathf.Abs(transform.position.x - target.transform.position.x) + Mathf.Abs(transform.position.y - target.transform.position.y))
                {
                    swingingPointAlt = target;
                }
            }
        }
    }

    private void ReturnPlayerState()
    {
        player.state = Player.State.Neutral;
        characterRigidBody.gravityScale = characterGravity;
        targetLaunchPosition = new Vector2(0,0);
        animator.SetBool("isScarfThrown", false);
    }

    private void DelayBeforeSwingStart()
    {
        if ((swingingPoint != null || swingingPointAlt != null) && !isSwinging && GameManager.Instance.redScarf)
        {
            ToggleIsSwinging();
        }
        if (targetLaunchPosition != null && isSwinging)
        {
            hasStartedSwing = true;
        }
        else
        {
            Invoke("ReturnPlayerState", 0.1f);
        }
    }
}
