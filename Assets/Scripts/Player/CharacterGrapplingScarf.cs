using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public GameObject swingingPoint;
    [SerializeField] Rigidbody2D characterRigidBody;
    [SerializeField] LayerMask grabableLayers;
    [SerializeField] float lengthOfScarf = 1f;
    [Header("Stats")]
    [SerializeField] float dashStrength = 1.5f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float gravityAdjustment = 0.5f;
    [SerializeField] float airDashCooldown = 0.2f;
    float originalAirDashCooldown;
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
        originalAirDashCooldown = airDashCooldown;
        airDashCooldown = 0;
    }

    private void Start()
    {
        animator.SetBool("stopScarfThrow", true);
    }

    void Update()
    {
        if (airDashCooldown <= 0 && theGrabbable != null && (InputManager.Instance.GetKeyDown(KeybindingActions.Special) || !player.grounded))
        {
            theGrabbable.HandleGrabbedTowards();
        }
        else if (airDashCooldown <= 0 && theGrabbable != null && ((InputManager.Instance.GetKeyDown(KeybindingActions.Right) && swingingPoint.transform.position.x > transform.position.x) || (InputManager.Instance.GetKeyDown(KeybindingActions.Left) && swingingPoint.transform.position.x < transform.position.x)))
        {
            theGrabbable.HandleGrabbedTowards();
        }
        else if (airDashCooldown <= 0 && theGrabbable != null && ((InputManager.Instance.GetKeyDown(KeybindingActions.Right) && swingingPoint.transform.position.x < transform.position.x) || (InputManager.Instance.GetKeyDown(KeybindingActions.Left) && swingingPoint.transform.position.x > transform.position.x)))
        {
            theGrabbable.HandleGrabbedAway();
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Special) && GameManager.Instance.redScarf && player.state == Player.State.Neutral && airDashCooldown <= 0)
        {
            if (player.state != Player.State.Dash)
            {
                animator.SetTrigger("startScarfThrow");
                player.state = Player.State.Dash;
                characterRigidBody.gravityScale = gravityAdjustment;
                Invoke("ReturnGravityAdjustments", dashDuration);
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
        }
        if (airDashCooldown > 0)
        {
            if (player.grounded)
            {
                airDashCooldown -= Time.deltaTime * 2;
            }
            else
            {
                airDashCooldown -= Time.deltaTime;
            }
        }
    }

    /*
    private void FixedUpdate()
    {
        if (hasStartedSwing && swingingPoint != null)
        {
            if (Vector2.Distance(transform.position, swingingPoint.transform.position) > Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + playerDistanceToStop && isReadyToStop)
            {
                ReturnPlayerState();
                CancelInvoke("ReturnPlayerState");
                CancelInvoke("InvokePlayerReadyToStop");
            }
        }
    }
    */

    private void ScarfThrowLocation()
    {
        float length = lengthOfScarf;
        float height = lengthOfScarf * 0.8f;
        float width = 1;
        if (!InputManager.Instance.GetKey(KeybindingActions.Up))
        {
            height = 0;
        }
        if (InputManager.Instance.GetKey(KeybindingActions.Up) && !(InputManager.Instance.GetKey(KeybindingActions.Right) || InputManager.Instance.GetKey(KeybindingActions.Left)))
        {
            length = 1;
        }
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(Vector2.zero, new Vector2(1, 1), 90f, grabableLayers);
        Vector2 targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);
        while (length > 0 && swingingPoint == null)
        {
            targetLocation = new Vector2(transform.position.x, transform.position.y);
            if (InputManager.Instance.GetKey(KeybindingActions.Up) && !(InputManager.Instance.GetKey(KeybindingActions.Right) || InputManager.Instance.GetKey(KeybindingActions.Left)))
            {
                targetLocation.y += height;
                width += 0.2f;
            }
            else
            {
                targetLocation.y += height;
                targetLocation.x += Mathf.Sign(transform.rotation.y);
                targetLocation.x += length * Mathf.Sign(transform.rotation.y);
            }
            if (InputManager.Instance.GetKey(KeybindingActions.Right) || InputManager.Instance.GetKey(KeybindingActions.Left))
            {
                targetLocation.x += Mathf.Sign(transform.rotation.y);
            }

            hitTargets = Physics2D.OverlapBoxAll(targetLocation, new Vector2(width, 1), 90f, grabableLayers);

            foreach (Collider2D target in hitTargets)
            {
                if (target != null)
                {
                    if (target.GetComponent<IGrabbable>() != null)
                    {
                        swingingPoint = target.gameObject;
                        theGrabbable = target.GetComponent<IGrabbable>();
                        target.GetComponent<IGrabbable>().IsGrabbed();
                    }
                }
            }
            if (length > 1 || height <= 0)
            {
                length--;
            }
            else
            {
                length = lengthOfScarf;
                height -= 1;
                if (height < 0)
                    height = 0;
            }
        }
        if (swingingPoint == null)
        {
            Invoke("ReturnPlayerState", dashDuration * 1.1f);
            return;
        }
        animator.SetBool("isScarfThrown", true);
        animator.SetBool("stopScarfThrow", false);
        ToggleIsSwinging();
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);
        if (InputManager.Instance.GetKey(KeybindingActions.Up) && !(InputManager.Instance.GetKey(KeybindingActions.Right) || InputManager.Instance.GetKey(KeybindingActions.Left)))
        {
            targetLocation.x -= Mathf.Sign(transform.rotation.y);
        }
        else
        {
            targetLocation.x += lengthOfScarf * Mathf.Sign(transform.rotation.y);
        }
        if (InputManager.Instance.GetKey(KeybindingActions.Up))
            targetLocation.y += lengthOfScarf * 0.8f;

        Gizmos.DrawLine(transform.position, targetLocation);
    }

    public void LaunchPlayerIntoDash()
    {
        airDashCooldown = originalAirDashCooldown;
        characterRigidBody.gravityScale = gravityAdjustment;
        CancelInvoke("ReturnGravityAdjustments");
        Invoke("ReturnGravityAdjustments", 0.6f * dashDuration);
        animator.Play("Scarf Pull Jump");
        characterRigidBody.velocity = Vector2.zero;
        float theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.85f, 1.1f);
        theBonusInStrength = Mathf.Round(theBonusInStrength * 100f) / 100f;

        characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * theBonusInStrength);

        CancelInvoke("ReturnPlayerStateState");
        CancelInvoke("ReturnPlayerStateStatus");
        Invoke("ReturnPlayerStateStatus", dashDuration);
        animator.SetBool("isScarfThrown", false);
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
            CancelInvoke("ReturnGravityAdjustments");
            Invoke("InvokePlayerReadyToStop", theDashDuration * 0.6f);
            Invoke("ReturnPlayerState", theDashDuration * 1.25f);
        }
    }

    private void InvokePlayerReadyToStop()
    {
        isReadyToStop = true;
    }

    public void ReturnPlayerState()
    {
        ReturnPlayerStateAnim();
        Invoke("ReturnPlayerStateStatus", 0.2f);
    }

    private void ReturnPlayerStateStatus()
    {
        if (IsInvoking("ReturnPlayerStateStatus"))
            CancelInvoke("ReturnPlayerStateStatus");

        isReadyToStop = false;
        player.state = Player.State.Neutral;
        characterRigidBody.gravityScale = characterGravity;
        originalLaunchPosition = new Vector2(transform.position.x, transform.position.y);
        swingingPoint = null;
        theGrabbable = null;
    }

    private void ReturnPlayerStateAnim()
    {
        if (IsInvoking("ReturnPlayerStateAnim"))
            CancelInvoke("ReturnPlayerStateAnim");
        animator.SetBool("isScarfThrown", false);
        animator.SetBool("stopScarfThrow", true);
    }

    private void DelayBeforeSwingStart()
    {
        if (swingingPoint == null && GameManager.Instance.redScarf)
        {
            ScarfThrowLocation();
        }
    }
    private void ReturnGravityAdjustments()
    {
        characterRigidBody.gravityScale = characterGravity;
    }
}
