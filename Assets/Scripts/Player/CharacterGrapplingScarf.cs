using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    [Header("Components")]
    bool isSwinging = false;
    bool hasStartedSwing = false;
    [SerializeField] public GameObject swingingPoint;
    [SerializeField] Rigidbody2D characterRigidBody;
    [Header("Stats")]
    [SerializeField] float dashStrength = 1.5f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float gravityAdjustment = 0.5f;
    [Header("Distance Stats")]
    [SerializeField] float closeDistanceThrow = 3f;
    [SerializeField] float playerDistanceToStop = 5f;

    float characterGravity = 1f;
    bool isReadyToStop = false;

    float theDistanceBias = 0;

    Vector2 originalLaunchPosition;
    Vector2 targetLaunchPosition;
    
    Player player;
    Animator animator;

    private void Awake()
    {
        if (GetComponent<Player>() && player == null)
        {
            player = GetComponent<Player>();
        }
        if (GetComponentInParent<Player>() && player == null)
        {
            player = GetComponentInParent<Player>();
        }
        if (GetComponent<Animator>() && animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (GetComponentInParent<Animator>() && animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }
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
                characterRigidBody.gravityScale = gravityAdjustment;
                Invoke("ReturnGravityAdjustments", 0.35f);
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
                    temp *= (0.2f / characterGravity);
                }
                else if (temp > 0.01f)
                {
                    temp += 0.1f * characterGravity;
                }
                characterRigidBody.velocity = new Vector2(characterRigidBody.velocity.x * 0.8f, temp);
            }

            //if ((swingingPoint != null || swingingPointAlt != null || swingingPoints[0] != null) && !isSwinging && GameManager.Instance.redScarf)
            //{
            //    ToggleIsSwinging();
            //}
        }
    }

    private void FixedUpdate()
    {
        if (hasStartedSwing && isSwinging && swingingPoint != null)
        {
            if (Vector2.Distance(transform.position, swingingPoint.transform.position) > Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + playerDistanceToStop && isReadyToStop)
            {
                ReturnPlayerState();
                isSwinging = false;
                CancelInvoke("ToggleIsSwinging");
                CancelInvoke("ReturnPlayerState");
                CancelInvoke("InvokePlayerReadyToStop");
            }
        }
    }
    
    private void LaunchPlayerIntoDash()
    {
        animator.SetTrigger("isJump");
        characterRigidBody.velocity = Vector2.zero;
        float theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.85f, 1.1f);
        theBonusInStrength = Mathf.Round(theBonusInStrength * 100f) / 100f;
        Debug.Log(theBonusInStrength);
        characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * theBonusInStrength);
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint, float extraDistanceBias)
    {
        swingingPoint = swingPoint;
        theDistanceBias = extraDistanceBias;
    }

    private void ToggleIsSwinging()
    {
        if (!isSwinging)
        {
            player.state = Player.State.Dash;
            originalLaunchPosition = transform.position;
        }
        isReadyToStop = false;
        hasStartedSwing = false;
        isSwinging = !isSwinging;
        if (swingingPoint != null)
        {
            swingingPoint.GetComponentInParent<SwingingPoint>().isSwingingFrom = isSwinging;
        }
        if (isSwinging)
        {
            if (swingingPoint != null)
            {
                targetLaunchPosition = swingingPoint.transform.position - this.transform.position;
                targetLaunchPosition.Normalize();
                originalLaunchPosition = this.transform.position;
            }
            characterRigidBody.gravityScale = gravityAdjustment;
            float theDashDuration = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow + 0.05f, 0.95f * dashDuration, 1.1f * dashDuration);
            theDashDuration = Mathf.Round(theDashDuration * 100f) / 100f;
            Debug.Log("Dash: " + theDashDuration);
            CancelInvoke("ToggleIsSwinging");
            CancelInvoke("ReturnPlayerState");
            CancelInvoke("InvokePlayerReadyToStop");
            CancelInvoke("ReturnGravityAdjustments");
            Invoke("InvokePlayerReadyToStop", theDashDuration * 0.6f);
            Invoke("ToggleIsSwinging", theDashDuration);
            Invoke("ReturnPlayerState", theDashDuration * 1.1f);
            LaunchPlayerIntoDash();
        }
    }

    private void InvokePlayerReadyToStop()
    {
        isReadyToStop = true;
    }

    private void ReturnPlayerState()
    {
        isReadyToStop = false;
        player.state = Player.State.Neutral;
        characterRigidBody.gravityScale = characterGravity;
        originalLaunchPosition = new Vector2(transform.position.x, transform.position.y);
        animator.SetBool("isScarfThrown", false);
    }

    private void DelayBeforeSwingStart()
    {
        if (swingingPoint != null && !isSwinging && GameManager.Instance.redScarf)
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
    private void ReturnGravityAdjustments()
    {
        characterRigidBody.gravityScale = characterGravity;
    }
}
