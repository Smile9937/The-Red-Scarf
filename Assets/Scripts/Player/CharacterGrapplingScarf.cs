using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrapplingScarf : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public GameObject swingingPoint;
    [SerializeField] Rigidbody2D characterRigidBody;
    [SerializeField] LayerMask grabableLayers;
    [SerializeField] LayerMask ground;
    [Header("Stats")]
    [SerializeField] float lengthOfScarf = 1f;
    [SerializeField] float dashStrength = 1.5f;
    [SerializeField] public float dashDuration = 1f;
    [SerializeField] float gravityAdjustment = 0.5f;
    [SerializeField] float airDashCooldown = 0.2f;
    [SerializeField] int dashAttackBonus = 10;
    float originalAirDashCooldown;
    [Header("Distance Stats")]
    [SerializeField] float closeDistanceThrow = 3f;

    float characterGravity = 1f;
    float theDistanceBias = 0;

    Vector2 originalLaunchPosition;
    Vector2 targetLaunchPosition;
    Vector2 targetLaunchPositionB;
    private bool grounded;
    private bool isDashing;

    Player player;
    RedScarfPlayer redScarfPlayer;
    [SerializeField] Animator animator;
    IGrabbable theGrabbable = null;


    [Header("Scarf GFX")]
    [SerializeField] private LineRenderer theLineRenderer;
    [SerializeField] LayerMask scarfGrabableLayers;
    [SerializeField] Transform scarfOriginLocation;
    Vector2 scarfGrabLocation;
    private RaycastHit2D hit;
    private bool grabbedNewLocation = false;
    [SerializeField] private ScarfGFXState scarfGFXState;
    [SerializeField]
    private enum ScarfGFXState
    {
        GrabbedState,
        PassedGrabbedState,
        NoGrabState,
    };

    [Header("Scarf Direction")]
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
        if (redScarfPlayer == null && FindObjectOfType<RedScarfPlayer>())
        {
            redScarfPlayer = FindObjectOfType<RedScarfPlayer>();
        }
        if (scarfOriginLocation != null)
            scarfGrabLocation = scarfOriginLocation.position;
        
        characterGravity = characterRigidBody.gravityScale;
        originalAirDashCooldown = airDashCooldown;
        airDashCooldown = 0;
    }

    private void Start()
    {
        Invoke("ReturnPlayerStateAnim", 0.01f);
    }

    void Update()
    {
        grounded = (Physics2D.OverlapBox(player.groundCheck.position, player.groundCheckSize, 0, ground) || player.grounded);

        if (theGrabbable != null)
        {
            if (airDashCooldown <= 0 && (InputManager.Instance.GetKeyDown(KeybindingActions.Special) || !grounded))
            {
                CancelInvoke("StopPlayerGliding");
                theGrabbable.HandleGrabbedTowards();
            }
            else
            {
                if (swingingPoint != null)
                {
                    if (airDashCooldown <= 0 && ((InputManager.Instance.GetKey(KeybindingActions.Right) && swingingPoint.transform.position.x > transform.position.x) || (InputManager.Instance.GetKey(KeybindingActions.Left) && swingingPoint.transform.position.x < transform.position.x)))
                    {
                        CancelInvoke("StopPlayerGliding");
                        theGrabbable.HandleGrabbedTowards();
                    }
                    else if (airDashCooldown <= 0 && theGrabbable != null && ((InputManager.Instance.GetKey(KeybindingActions.Right) && swingingPoint.transform.position.x < transform.position.x) || (InputManager.Instance.GetKey(KeybindingActions.Left) && swingingPoint.transform.position.x > transform.position.x)))
                    {
                        CancelInvoke("StopPlayerGliding");
                        theGrabbable.HandleGrabbedAway();
                    }
                }
            }
        }
        if (player.state == Player.State.Neutral)
        {
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
        if (isDashing)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack) && player.hasBaseballBat)
            {
                DoDashAttack();
            }
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Special) && GameManager.Instance.redScarf && player.state == Player.State.Neutral && airDashCooldown <= 0)
        {
            if (player.state != Player.State.Dash)
            {
                animator.SetTrigger("startScarfThrow");
                animator.SetBool("stopScarfThrow", false);
                grabbedNewLocation = false;

                player.state = Player.State.Dash;
                characterRigidBody.gravityScale = gravityAdjustment;
                Invoke("ReturnGravityAdjustments", dashDuration);
                if (theLineRenderer != null)
                {
                    theLineRenderer.SetPosition(0, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                    theLineRenderer.SetPosition(1, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                }
            }
            if (grounded)
            {
                LowerPlayerSpeed();
                float theTimeToThrow = 0.85f;
                theTimeToThrow -= 0.05f * Mathf.Clamp(animator.GetFloat("axisXSpeed"), 0.5f,2);
                Invoke("ToggleIsSwinging", dashDuration * theTimeToThrow);
            }
            else
            {
                float temp = characterRigidBody.velocity.y;
                temp *= gravityAdjustment;
                if (temp >= 0.01f)
                {
                    temp -= 0.1f * characterGravity;
                }
                characterRigidBody.velocity = new Vector2(characterRigidBody.velocity.x * 0.8f, temp);
            }
        }
        if (airDashCooldown > 0)
        {
            if (grounded)
            {
                airDashCooldown -= Time.deltaTime * 2;
            }
            else
            {
                airDashCooldown -= Time.deltaTime;
            }
        }
        UpdateOfRenderedScarf();
    }

    private void FixedUpdate()
    {
        if (scarfGFXState == ScarfGFXState.GrabbedState)
        {
            if (originalLaunchPosition.x < targetLaunchPositionB.x && transform.position.x >= targetLaunchPositionB.x)
            {
                scarfGFXState = ScarfGFXState.PassedGrabbedState;
                ReturnHookedPointGFX();
            }
            else if (originalLaunchPosition.x > targetLaunchPositionB.x && transform.position.x <= targetLaunchPositionB.x)
            {
                scarfGFXState = ScarfGFXState.PassedGrabbedState;
                ReturnHookedPointGFX();
            }
            else if (originalLaunchPosition.y < targetLaunchPositionB.y && transform.position.y >= targetLaunchPositionB.y)
            {
                scarfGFXState = ScarfGFXState.PassedGrabbedState;
                ReturnHookedPointGFX();
            }
            else if (originalLaunchPosition.y > targetLaunchPositionB.y && transform.position.y <= targetLaunchPositionB.y)
            {
                scarfGFXState = ScarfGFXState.PassedGrabbedState;
                ReturnHookedPointGFX();
            }
        }
    }

    private void DoDashAttack()
    {
        isDashing = false;
        ReturnHookedPointGFX();
        scarfGFXState = ScarfGFXState.PassedGrabbedState;
        player.attackBonus += dashAttackBonus;

        player.gameObject.layer = LayerMask.NameToLayer("Dodge Roll");

        player.state = Player.State.Attacking;
        player.myRigidbody.velocity = new Vector2(player.myRigidbody.velocity.x * 0.9f, player.myRigidbody.velocity.y);

        redScarfPlayer.attackAreaMultiplier = 1.25f;

        animator.Play("DashAttack");
        player.nextMeleeAttackTime = Time.time + 1f / player.meleeAttackRate;

        characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * 0.25f);

        player.state = Player.State.Dash;
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
        CancelInvoke("ReturnPlayerState");
        if (swingingPoint == null)
        {
            animator.SetBool("stopScarfThrow", true);
            Invoke("ReturnPlayerState", dashDuration * 1.1f);
            return;
        }
        animator.SetBool("isScarfThrown", true);
        if (grounded)
        {
            Invoke("StopPlayerGliding", Mathf.Clamp(Vector2.Distance(swingingPoint.transform.position, this.transform.position) * 0.02f, 0.25f, 1.5f));
        }
        grabbedNewLocation = true;
        scarfGrabLocation = swingingPoint.transform.position;
        ToggleIsSwinging();
    }

    private void LowerPlayerSpeed()
    {
        characterRigidBody.velocity = new Vector2(characterRigidBody.velocity.x * 0.8f, characterRigidBody.velocity.y);
    }

    public void LaunchPlayerIntoDash()
    {
        if (swingingPoint != null && airDashCooldown <= 0)
        {
            isDashing = true;
            airDashCooldown = originalAirDashCooldown;
            targetLaunchPosition = swingingPoint.transform.position - this.transform.position;
            targetLaunchPositionB = swingingPoint.transform.position;
            targetLaunchPosition.Normalize();

            characterRigidBody.gravityScale = gravityAdjustment;
            CancelInvoke("ReturnGravityAdjustments");
            Invoke("ReturnGravityAdjustments", 0.6f * dashDuration);
            animator.Play("Scarf Pull Jump");
            characterRigidBody.velocity = Vector2.zero;
            float theBonusInStrength = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow, 0.85f, 1.1f);

            characterRigidBody.AddForce(targetLaunchPosition * 9.82f * dashStrength * theBonusInStrength);

            CancelInvoke("ReturnPlayerState");
            CancelInvoke("ReturnPlayerStateStatus");
            Invoke("ReturnPlayerStateStatus", dashDuration * 1.1f);

            ReturnHookedPointGFX();

            swingingPoint = null;
            theGrabbable = null;
            animator.SetBool("isScarfThrown", false);
            animator.SetBool("stopScarfThrow", true);
        }
    }

    public void SetSwingingPointAsTarget(GameObject swingPoint, float extraDistanceBias)
    {
        swingingPoint = swingPoint;
        theDistanceBias = extraDistanceBias;
    }

    private void ToggleIsSwinging()
    {
        CancelInvoke("ReturnPlayerState");
        originalLaunchPosition = transform.position;
        if (swingingPoint != null)
        {
            scarfGFXState = ScarfGFXState.GrabbedState;
            characterRigidBody.gravityScale = gravityAdjustment;
            
            float theDashDuration = Mathf.Clamp((Vector2.Distance(originalLaunchPosition, swingingPoint.transform.position) + theDistanceBias) / closeDistanceThrow + 0.05f, 0.9f * dashDuration, 1.1f * dashDuration);
            theDashDuration = Mathf.Round(theDashDuration * 100f) / 100f;
            
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

    public void PlayScarfPullAnimation()
    {
        animator.Play("Scarf Pull Jump");
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
        isDashing = false;
        grabbedNewLocation = false;
        scarfGFXState = ScarfGFXState.PassedGrabbedState;
    }

    private void ReturnPlayerStateAnim()
    {
        if (IsInvoking("ReturnPlayerStateAnim"))
            CancelInvoke("ReturnPlayerStateAnim");

        animator.SetBool("isScarfThrown", false);
        animator.SetBool("stopScarfThrow", true);

        if (!IsInvoking("ReturnPlayerStateStatus"))
            Invoke("ReturnPlayerStateStatus", 0.2f);
    }

    private void DelayBeforeSwingStart()
    {
        if (swingingPoint == null && GameManager.Instance.redScarf)
        {
            ScarfThrowLocation();
        }
        
         CancelInvoke("ReturnPlayerState");
         Invoke("ReturnPlayerState", Mathf.Clamp(dashDuration,0.5f,3f));
    }
    private void ReturnGravityAdjustments()
    {
        characterRigidBody.gravityScale = characterGravity;
    }
    private void ReturnAttackBonus()
    {
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        player.attackBonus -= dashAttackBonus;

        redScarfPlayer.attackAreaMultiplier = 1;

        Invoke("ReturnPlayerState", 0.1f);
    }
    
    private void StopPlayerGliding()
    {
        if (IsInvoking("StopPlayerGliding"))
            CancelInvoke("StopPlayerGliding");
        characterRigidBody.velocity = Vector2.zero;
    }
    

    // Scarf GFX
    private void ActivateScarfRenderer(bool enabled)
    {
        if (theLineRenderer == null)
            return;
    }

    private void ReturnHookedPointGFX()
    {
        if (swingingPoint != null)
        {
            SwingingPoint theSwingingPoint = swingingPoint.GetComponent<SwingingPoint>();

            if (theSwingingPoint != null)
                theSwingingPoint.ReturnSwingingPointSprite();
        }
    }

    protected void UpdateOfRenderedScarf()
    {
        switch (scarfGFXState)
        {
            case(ScarfGFXState.NoGrabState):
                break;
            case (ScarfGFXState.GrabbedState):
                if (swingingPoint != null)
                {
                    grabbedNewLocation = true;
                    scarfGrabLocation = swingingPoint.transform.position;
                }

                hit = Physics2D.Raycast(scarfOriginLocation.position, scarfGrabLocation, lengthOfScarf * 3f, scarfGrabableLayers);

                if (hit)
                {
                    theLineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, 0));
                }
                else
                {
                    if (Vector2.Distance(transform.position, originalLaunchPosition) >= 3)
                    {
                        theLineRenderer.SetPosition(1, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                        grabbedNewLocation = false;
                    }
                    else if (grabbedNewLocation)
                    {
                        theLineRenderer.SetPosition(1, new Vector3(scarfGrabLocation.x, scarfGrabLocation.y, 0));
                    }
                    else
                    {
                        theLineRenderer.SetPosition(1, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                    }
                }

                theLineRenderer.SetPosition(0, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                break;
            default:
                theLineRenderer.SetPosition(0, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                theLineRenderer.SetPosition(1, new Vector3(scarfOriginLocation.position.x, scarfOriginLocation.position.y, 0));
                break;
        }
    }
}
