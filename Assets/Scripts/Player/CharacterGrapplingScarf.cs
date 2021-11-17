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
    float theDistanceBias = 0;

    Vector2 originalLaunchPosition;
    Vector2 targetLaunchPosition;

    Player player;
    Animator animator;
    IGrabbable theGrabbable = null;


    [Header("Enemy Scarf Interaction")]
    [SerializeField] private ScarfDirectionEnum scarfDirection;
    [SerializeField] private enum ScarfDirectionEnum
    {
        Uppwards,
        Sideways,
        UppSideways,
    };


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
                if (InputManager.Instance.GetKey(KeybindingActions.Up) && !InputManager.Instance.GetKey(KeybindingActions.Left) && !InputManager.Instance.GetKey(KeybindingActions.Right))
                {
                    scarfDirection = ScarfDirectionEnum.Uppwards;
                }
                else if (InputManager.Instance.GetKey(KeybindingActions.Up) && (InputManager.Instance.GetKey(KeybindingActions.Left) || InputManager.Instance.GetKey(KeybindingActions.Right)))
                {
                    scarfDirection = ScarfDirectionEnum.UppSideways;
                }
                else
                {
                    scarfDirection = ScarfDirectionEnum.Sideways;
                }
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

    private void ScarfThrowLocation()
    {
        float length = lengthOfScarf;
        float height = lengthOfScarf * 0.8f;
        float hitBoxWidth = 1;
        float hitBoxHeight = 1;

        int curretNumOfTries = 0;
        float maxNumOfTries = length * height;
        if (scarfDirection == ScarfDirectionEnum.Sideways)
        {
            height = 0;
        }
        if (scarfDirection == ScarfDirectionEnum.Uppwards)
        {
            length = 1;
        }

        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(Vector2.zero, new Vector2(1, 1), 90f, grabableLayers);
        Vector2 targetLocation = new Vector2(transform.position.x + Mathf.Sign(transform.rotation.y), transform.position.y);

        while (length >= 0 && swingingPoint == null && curretNumOfTries < maxNumOfTries)
        {
            targetLocation = new Vector2(transform.position.x + (0.9f * Mathf.Sign(transform.rotation.y)), transform.position.y);
            if (length == 0)
                targetLocation.x += 0.25f * Mathf.Sign(transform.rotation.y);
            if (scarfDirection == ScarfDirectionEnum.Uppwards)
            {
                targetLocation.y += height;
                hitBoxWidth += 0.1f;
            }
            else
            {
                targetLocation.y += height;
                targetLocation.x += length * Mathf.Sign(transform.rotation.y);
            }
            if (scarfDirection == ScarfDirectionEnum.Sideways)
            {
                hitBoxHeight += 0.05f;
                Mathf.Clamp(hitBoxHeight, 1, 1.5f);
            }

            hitTargets = Physics2D.OverlapBoxAll(targetLocation, new Vector2(hitBoxWidth, hitBoxHeight), 90f, grabableLayers);

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
            curretNumOfTries++;
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
    /*
    private void OnDrawGizmosSelected()
    {
        Vector2 targetLocation = new Vector2(transform.position.x, transform.position.y);
        Vector2 originalLocation = new Vector2(transform.position.x + (0.1f * Mathf.Sign(transform.rotation.y)), transform.position.y);
        targetLocation.x += lengthOfScarf * Mathf.Sign(transform.rotation.y);
        if (InputManager.Instance.GetKey(KeybindingActions.Up))
        {
            targetLocation.y += lengthOfScarf * 0.8f;
            if (!(InputManager.Instance.GetKey(KeybindingActions.Right) || InputManager.Instance.GetKey(KeybindingActions.Left)))
            {
                targetLocation.x = transform.position.x;
            }
        }

        Gizmos.DrawLine(originalLocation, targetLocation);
    }
    */

    public void LaunchPlayerIntoDash()
    {
        if (swingingPoint != null)
        {
            airDashCooldown = originalAirDashCooldown;
            characterRigidBody.gravityScale = gravityAdjustment;
            CancelInvoke("ReturnGravityAdjustments");
            Invoke("ReturnGravityAdjustments", 0.6f * dashDuration);
            animator.Play("Scarf Pull Jump");
            characterRigidBody.velocity = Vector2.zero;
            float theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.85f, 1.1f);

            characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * theBonusInStrength);

            theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.9f, 1.1f);
            theBonusInStrength = Mathf.Round(theBonusInStrength * 100f) / 100f;

            //if (Vector2.Distance(originalLaunchPosition, transform.position) <= 4)
            //    CancelInvoke("ReturnPlayerStateStatus");

            Invoke("ReturnPlayerStateStatus", dashDuration * theBonusInStrength);
            animator.SetBool("isScarfThrown", false);
            swingingPoint = null;
            theGrabbable = null;
        }
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint, float extraDistanceBias)
    {
        swingingPoint = swingPoint;
        theDistanceBias = extraDistanceBias;
        if (swingingPoint == null)
            theGrabbable = null;
    }

    private void ToggleIsSwinging()
    {
        originalLaunchPosition = transform.position;
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
            Invoke("ReturnPlayerState", theDashDuration * 1.25f);
        }
    }

    public void ReturnPlayerState()
    {
        if (IsInvoking("ReturnPlayerState"))
            CancelInvoke("ReturnPlayerState");
        ReturnPlayerStateAnim();
    }

    private void ReturnPlayerStateStatus()
    {
        if (IsInvoking("ReturnPlayerStateStatus"))
            CancelInvoke("ReturnPlayerStateStatus");

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
