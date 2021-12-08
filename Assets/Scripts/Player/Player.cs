using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, ICharacter
{
    private float nextGroundSlamAttackTime = 0f;

    [HideInInspector]
    public float nextMeleeAttackTime = 0f;

    [Header("Movement Variables")]
    private float direction;

    private bool facingRight = true;

    [Header("Jump Variables")]
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    [SerializeField] private LayerMask ground;
    public bool grounded;
    [SerializeField] private float offGroundJumpTimer = 0.1f;
    private float jumpTimeCounter;
    private bool stoppedJumping = true;

    private bool countDownCanJumpTimer;

    private bool previouslyGrounded;

    [Header("Character Status")]
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float secondsOfInvincibility;
    public bool isInvincible = false;

    private bool canJump = false;

    private float canJumpCounter;

    private float knockbackCount;
    private bool knockedFromRight;

    private Vector2 knockback;

    public float meleeAttackRate;
    public bool hasBaseballBat;
    private float oldPos;

    public State state;
    public enum State
    {
        Neutral,
        Attacking,
        Rolling,
        Blocking,
        Dash,
        GroundSlam,
        Dead,
        ReturningToNeutral
    }

    [Header("Animator Controllers")]
    [SerializeField] private RuntimeAnimatorController redScarfUnarmedAnimator;
    [SerializeField] public RuntimeAnimatorController redScarfBaseballbatController;
    [SerializeField] private RuntimeAnimatorController dressAnimator;

    [HideInInspector]
    public BoxCollider2D myCollider;
    [HideInInspector]
    public CircleCollider2D rollCollider;
    [HideInInspector]
    public Rigidbody2D myRigidbody;
    [HideInInspector]
    public Animator myAnimator;

    [Header("Components")]
    public Transform attackPoint;
    public LayerMask attackLayers;
    public DamagePopUp damageText;
    [SerializeField] private LayerMask groundSlamLayer;

    public RedScarfPlayer redScarf;
    public DressPlayer dress;

    public PlayerStats redScarfStats;
    public PlayerStats dressStats;

    private PlayerStats currentPlayerStats;

    public float speedBonus;
    public int attackBonus;

    [SerializeField] Checkpoint checkpoint;

    [SerializeField] private Animator squashAndStretch;

    private const string SQUASH = "Squash";
    private const string STRETCH = "Stretch";
    private const string STRETCH_RETURN = "StretchReturn";
    private const string BIG_SQUASH = "BigSquash";

    private string currentSquashAndStretchAnimation;

    public SoundPlayer soundPlayer;

    private void OnDisable()
    {
        GameEvents.Instance.onSaveGame -= Save;
        GameEvents.Instance.onLoadGame -= Load;
    }
    private void Save()
    {
        GameManager.Instance.hasBaseballBat = hasBaseballBat;
        GameManager.Instance.playerAtkBonus = attackBonus;
    }
    private void Load()
    {
        hasBaseballBat = GameManager.Instance.hasBaseballBat;
        attackBonus = GameManager.Instance.playerAtkBonus;
        SetCurrentCharacter();
    }
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
        rollCollider = GetComponent<CircleCollider2D>();
        soundPlayer = GetComponent<SoundPlayer>();

        GameEvents.Instance.onSaveGame += Save;
        GameEvents.Instance.onLoadGame += Load;
    }
    private void Start()
    {
        canJumpCounter = offGroundJumpTimer;
        currentHealth = maxHealth;

        transform.position = GameManager.Instance.currentSpawnpoint;
        state = State.Neutral;

        rollCollider.enabled = false;

        SetCurrentCharacter();
        jumpTimeCounter = currentPlayerStats.jumpTime;
    }

    private void Update()
    {
        if (PauseMenu.Instance.gamePaused)
        {
            return;
        }

        if (InputManager.Instance.GetKeyDown(KeybindingActions.PlaceCheckpoint))
        {
            Instantiate(checkpoint, transform.position, Quaternion.Euler(0, 0, 90));
        }

        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, ground);

        PlayerUI.Instance.SetHealthUI(currentHealth, maxHealth);
        myAnimator.SetBool("isGrounded", grounded);

        if (InputManager.Instance.GetKey(KeybindingActions.Left)
            && !InputManager.Instance.GetKey(KeybindingActions.Right))
        {
            direction = -1;
        }
        else if (InputManager.Instance.GetKey(KeybindingActions.Right) &&
            !InputManager.Instance.GetKey(KeybindingActions.Left))
        {
            direction = 1;
        }
        else
        {
            direction = 0;
        }


        switch (state)
        {
            case State.Neutral:
                HandleJumping();
                HandleMovement();
                SwapCharacter();
                //HandleGroundSlam();
                break;
            case State.Blocking:
                break;
            case State.Dash:
                //Block();
                myAnimator.SetFloat("axisYSpeed", Mathf.Clamp(myRigidbody.velocity.y, -1, 1));
                break;
            //case State.GroundSlam:
                //HandleGroundSlam();
                //break;
        }

        if (grounded)
        {
            jumpTimeCounter = currentPlayerStats.jumpTime;
        }
        if (countDownCanJumpTimer)
        {
            jumpTimeCounter -= Time.deltaTime;
        }
        CheckIfCanJump();
    }
    private void FixedUpdate()
    {
        if (knockbackCount > 0)
        {
            HandleKnockBack();
        }

        if(state == State.Neutral && knockbackCount <= 0)
        {
            HandleMovement();
        }
    }
    public void SetCurrentCharacter()
    {
        if (GameManager.Instance.redScarf)
        {
            currentPlayerStats = redScarfStats;
            redScarf.gameObject.SetActive(true);
            dress.gameObject.SetActive(false);
        }
        else
        {
            currentPlayerStats = dressStats;
            dress.gameObject.SetActive(true);
            redScarf.gameObject.SetActive(false);
        }
        meleeAttackRate = currentPlayerStats.meleeAttackRate;
        SetAnimator();
    }
    private void SetAnimator()
    {
        if (hasBaseballBat && GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfBaseballbatController; }
        else if (GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfUnarmedAnimator; }
        else { myAnimator.runtimeAnimatorController = dressAnimator; }
    }
    private void SwapCharacter()
    {
        if (InputManager.Instance.GetKeyDown(KeybindingActions.SwapCharacter))
        {
            GameManager.Instance.SwapCharacter();
            SetCurrentCharacter();
            SetAnimator();
        }
    }

    private void CheckIfCanJump()
    {
        if (!grounded && stoppedJumping)
        {
            canJumpCounter -= Time.deltaTime;
        }

        if (grounded)
        {
            canJumpCounter = offGroundJumpTimer;
            canJump = true;
        }

        if (canJumpCounter <= 0)
        {
            canJump = false;
        }

        if (!grounded && !stoppedJumping)
        {
            canJump = false;
        }
    }

    private void HandleKnockBack()
    {
        if (knockedFromRight)
        {
            myRigidbody.velocity = new Vector2(-knockback.x, knockback.y);
        }
        else if (!knockedFromRight)
        {
            myRigidbody.velocity = new Vector2(knockback.x, knockback.y);
        }
        knockbackCount -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        myAnimator.SetFloat("axisXSpeed", Mathf.Abs(direction));
        TurnAround(direction);
        Move();
    }

    private void Move()
    {
        myRigidbody.velocity = new Vector2(direction * (currentPlayerStats.speed + speedBonus), myRigidbody.velocity.y);
    }

    private void TurnAround(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    private void HandleJumping()
    {
        if (grounded)
        {
            myAnimator.SetFloat("axisYSpeed", 0);
            countDownCanJumpTimer = false;
        }
        else
        {
            myAnimator.SetFloat("axisYSpeed", Mathf.Clamp(myRigidbody.velocity.y, -1, 1));
        }

        if (InputManager.Instance.GetKeyDown(KeybindingActions.Jump) && canJump)
        {
            Jump();
            PlaySquashAndStretchAnimation(STRETCH);
            stoppedJumping = false;
        }

        if (InputManager.Instance.GetKey(KeybindingActions.Jump) && !stoppedJumping && jumpTimeCounter > 0 && !grounded)
        {
            Jump();
            countDownCanJumpTimer = true;
        }

        if (InputManager.Instance.GetKeyUp(KeybindingActions.Jump))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
        }

        if(grounded && !previouslyGrounded)
        {
            PlaySquashAndStretchAnimation(SQUASH);
        }

        previouslyGrounded = grounded;
    }

    private void Jump()
    {
        myAnimator.SetTrigger("isJump");
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, currentPlayerStats.jumpForce);
    }
    public void Die()
    {
        state = State.Dead;
        myRigidbody.velocity = Vector2.zero;
        myAnimator.SetBool("isDead", true);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.RespawnPlayer();
    }
    public void Damage(int damage, bool bypassInvincibility)
    {
        if (isInvincible && !bypassInvincibility)
            return;

        if (state == State.Blocking)
            return;

        currentHealth -= damage;
        PlayerUI.Instance.SetHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(InvincibilityFrames());
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        yield return new WaitForSeconds(secondsOfInvincibility);

        isInvincible = false;
    }

    public void KnockBack(GameObject knockbackSource, Vector2 knockbackVelocity, float knockbackLength)
    {
        if (isInvincible)
            return;

        knockback = knockbackVelocity;
        knockbackCount = knockbackLength;

        state = State.ReturningToNeutral;
        myAnimator.SetTrigger("isStaggered");

        if (transform.position.x < knockbackSource.transform.position.x)
        {
            knockedFromRight = true;
        }
        else
        {
            knockedFromRight = false;
        }
    }

    private void HandleGroundSlam()
    {
        if (Time.time >= nextGroundSlamAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack) && InputManager.Instance.GetKey(KeybindingActions.Down) && !grounded)
            {
                state = State.GroundSlam;
                myRigidbody.velocity = new Vector2(0, -20);
                gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
                nextGroundSlamAttackTime = Time.time + 1f / currentPlayerStats.groundSlamAttackRate;
            }
        }

        if (state == State.GroundSlam && grounded)
        {
            state = State.Neutral;
            gameObject.layer = LayerMask.NameToLayer("Player");
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(transform.position, currentPlayerStats.groundSlamArea, 0f, groundSlamLayer);

            foreach (Collider2D target in hitTargets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(currentPlayerStats.groundSlamDamage, false);

                    ICharacter character = target.GetComponent<ICharacter>();
                    if (character != null)
                    {
                        character.KnockBack(gameObject, currentPlayerStats.groundSlamKnockbackVelocity, currentPlayerStats.groundslamKnockbackLength);
                        Instantiate(damageText, target.transform.position, Quaternion.identity);
                        damageText.SetText(currentPlayerStats.groundSlamDamage);
                    }
                }
            }
        }
    }

    private void MeleeAttack()
    {
        if(GameManager.Instance.redScarf)
        {
            redScarf.MeleeAttack();
        }
        else
        {
            dress.MeleeAttack();
        }
    }

    private void ReturnFromAttack()
    {
        state = State.Neutral;
    }
    public void GainBaseballBat()
    {
        state = State.Neutral;
        hasBaseballBat = true;
        myAnimator.runtimeAnimatorController = redScarfBaseballbatController;
    }

    private void StopRollAnimation()
    {
        myAnimator.SetBool("isDodge", false);
    }
    private void StopRoll()
    {
        redScarf.StopRoll();
    }

    private void ReturnFromRolling()
    {
        redScarf.ReturnFromRolling();
    }

    private void ReturnFromStaggering()
    {
        state = State.Neutral;
    }

    private void PlayFootstepSound()
    {
        if (oldPos == transform.position.x)
            return;
        soundPlayer.PlaySound(0);
        //AudioLibrary.Instance.PlayOneShot("event:/SFX/Footsteps");
        oldPos = transform.position.x;
    }

    protected void PlaySquashAndStretchAnimation(string newAnimation)
    {
        if (currentSquashAndStretchAnimation == newAnimation)
            return;

        squashAndStretch.Play(newAnimation);

        currentSquashAndStretchAnimation = newAnimation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        if (attackPoint == null)
            return;
        //Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
