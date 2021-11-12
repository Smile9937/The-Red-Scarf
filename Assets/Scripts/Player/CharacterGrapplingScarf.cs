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
    [SerializeField] LayerMask grabableLayers;
    [SerializeField] float lengthOfScarf = 1f;
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

    IGrabbable theGrabbable = null;

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
    }

    void Update()
    {
        if (theGrabbable != null && (InputManager.Instance.GetKeyDown(KeybindingActions.Special) || !player.grounded))
        {
            theGrabbable.HandleGrabbed();
        }
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
            ScarfThrowLocation();
        }
    }

    private void FixedUpdate()
    {
        if (hasStartedSwing && swingingPoint != null)
        {
            if (Vector2.Distance(transform.position, swingingPoint.transform.position) > Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + playerDistanceToStop && isReadyToStop)
            {
                ReturnPlayerState();
                CancelInvoke("ToggleIsSwinging");
                CancelInvoke("ReturnPlayerState");
                CancelInvoke("InvokePlayerReadyToStop");
            }
        }
    }

    private void ScarfThrowLocation()
    {
        float length = lengthOfScarf;
        float height = lengthOfScarf * 0.8f;
        Vector2 targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);
        while (length > 0 && swingingPoint == null)
        {
            targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);
            if (InputManager.Instance.GetKey(KeybindingActions.Up))
                targetLocation.y += height;
            targetLocation.x += length * Mathf.Sign(transform.rotation.y);

            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(targetLocation, new Vector2(1, 1), 90f, grabableLayers);

            foreach (Collider2D target in hitTargets)
            {
                if (target != null)
                {
                    Debug.Log(target);
                    if (target.GetComponent<IGrabbable>() != null)
                    {
                        swingingPoint = target.gameObject;
                        theGrabbable = target.GetComponent<IGrabbable>();
                        target.GetComponent<IGrabbable>().IsGrabbed();
                    }
                }
            }
            Debug.Log("l " + length + " h " + height);
            Debug.Log(targetLocation);
            if (length > 1 || height <= 0)
            {
                length--;
            }
            else
            {
                length = lengthOfScarf;
                height -= 1;
            }
        }
        if (swingingPoint == null)
        {
            ReturnPlayerState();
            return;
        }
        ToggleIsSwinging();
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);
        targetLocation.x += lengthOfScarf * Mathf.Sign(transform.rotation.y);
        if (InputManager.Instance.GetKey(KeybindingActions.Up))
            targetLocation.y += lengthOfScarf * 0.8f;

        Gizmos.DrawLine(transform.position, targetLocation);
        //Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }

    public void LaunchPlayerIntoDash()
    {
        characterRigidBody.gravityScale = gravityAdjustment;
        CancelInvoke("ReturnGravityAdjustments");
        Invoke("ReturnGravityAdjustments", 0.35f);
        animator.SetTrigger("isJump");
        characterRigidBody.velocity = Vector2.zero;
        float theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.85f, 1.1f);
        theBonusInStrength = Mathf.Round(theBonusInStrength * 100f) / 100f;

        characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * theBonusInStrength);

        Invoke("ReturnPlayerState", dashDuration);
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint, float extraDistanceBias)
    {
        swingingPoint = swingPoint;
        theDistanceBias = extraDistanceBias;
    }

    private void ToggleIsSwinging()
    {
        originalLaunchPosition = transform.position;
        isReadyToStop = false;
        if (swingingPoint != null)
        {
            targetLaunchPosition = swingingPoint.transform.position - this.transform.position;
            targetLaunchPosition.Normalize();
            originalLaunchPosition = this.transform.position;
            characterRigidBody.gravityScale = gravityAdjustment;
            
            float theDashDuration = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow + 0.05f, 0.9f * dashDuration, 1.1f * dashDuration);
            theDashDuration = Mathf.Round(theDashDuration * 100f) / 100f;

            CancelInvoke("ReturnPlayerState");
            CancelInvoke("InvokePlayerReadyToStop");
            CancelInvoke("ReturnGravityAdjustments");
            Invoke("InvokePlayerReadyToStop", theDashDuration * 0.6f);
            Invoke("ReturnPlayerState", theDashDuration * 1.1f);
        }
    }

    private void InvokePlayerReadyToStop()
    {
        isReadyToStop = true;
    }

    public void ReturnPlayerState()
    {
        isReadyToStop = false;
        player.state = Player.State.Neutral;
        characterRigidBody.gravityScale = characterGravity;
        originalLaunchPosition = new Vector2(transform.position.x, transform.position.y);
        swingingPoint = null;
        theGrabbable = null;
        animator.SetBool("isScarfThrown", false);
    }

    private void DelayBeforeSwingStart()
    {
        if (swingingPoint != null && !isSwinging && GameManager.Instance.redScarf)
        {
            ScarfThrowLocation();
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
