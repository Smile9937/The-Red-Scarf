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
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask ground;
    public bool grounded;
    [SerializeField] private float offGroundJumpTimer = 0.1f;
    private float jumpTimeCounter;
    private bool stoppedJumping = true;

    [Header("Character Status")]
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float secondsOfInvincibility;
    bool isInvincible = false;

    private bool canJump = false;

    private float canJumpCounter;

    private float knockbackCount;
    private bool knockedFromRight;

    private Vector2 knockback;

    public float meleeAttackRate;
    public bool hasBaseballBat;

    //[HideInInspector]
    public State state;
    public enum State
    {
        Neutral,
        Rolling,
        Blocking,
        Dash,
        GroundSlam,
        Dead,
    };

    /*[Serializable]
    public class PlayerStats
    {
        public PlayerCharacterEnum playerCharacter;
        public float speed;
        public float jumpForce;
        public float jumpTime;
    }*/

    //public PlayerStats[] playerCharacters;

    //[HideInInspector] public PlayerStats currentCharacter;

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
    [HideInInspector] public Animator myAnimator;

    [Header("Components")]
    public Transform attackPoint;
    public LayerMask targetLayers;
    public DamagePopUp damageText;
    [SerializeField] private LayerMask groundSlamLayer;

    public RedScarfPlayer redScarf;
    public DressPlayer dress;

    public PlayerStats redScarfStats;
    public PlayerStats dressStats;

    private PlayerStats currentPlayerStats;

    private GameObject currentPlayer;
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
        rollCollider = GetComponent<CircleCollider2D>();

        canJumpCounter = offGroundJumpTimer;
        currentHealth = maxHealth;

        GameManager.Instance.player = this;
        //GameManager.Instance.LoadPlayerStats();


        transform.position = GameManager.Instance.currentSpawnpoint;
        state = State.Neutral;

        rollCollider.enabled = false;
        GameManager.Instance.LoadPlayerStats();

        SetCurrentCharacter();
        jumpTimeCounter = currentPlayerStats.jumpTime;
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

    private void Update()
    {
        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, ground);

        HandleGroundSlam();

        CheckIfCanJump();

        PlayerUI.Instance.SetHealthText(currentHealth);
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
                break;
            case State.Blocking:
                break;
            case State.Dash:
                //Block();
                myAnimator.SetFloat("axisYSpeed", Mathf.Clamp(myRigidbody.velocity.y, -1, 1));
                break;
        }

        //Swap Character
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
        myRigidbody.velocity = new Vector2(direction * currentPlayerStats.speed, myRigidbody.velocity.y);
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
            jumpTimeCounter = currentPlayerStats.jumpTime;
            myAnimator.SetFloat("axisYSpeed", 0);
        }
        else
        {
            myAnimator.SetFloat("axisYSpeed", Mathf.Clamp(myRigidbody.velocity.y, -1, 1));
        }

        if (InputManager.Instance.GetKeyDown(KeybindingActions.Jump) && canJump)
        {
            Jump();
            stoppedJumping = false;
        }

        if (InputManager.Instance.GetKey(KeybindingActions.Jump) && !stoppedJumping && jumpTimeCounter > 0 && !grounded)
        {
            Jump();
            jumpTimeCounter -= Time.deltaTime;
        }

        if (InputManager.Instance.GetKeyUp(KeybindingActions.Jump))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
        }
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
        currentHealth -= damage;

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
        knockback = knockbackVelocity;
        knockbackCount = knockbackLength;

        if (transform.position.x < knockbackSource.transform.position.x)
        {
            knockedFromRight = true;
        }
        else
        {
            knockedFromRight = false;
        }
    }

    private void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(attackPoint.position, currentPlayerStats.attackSize, 90f, targetLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                ICharacter character = target.GetComponent<ICharacter>();

                if (character != null)
                {
                    character.KnockBack(gameObject, currentPlayerStats.knockbackVelocity, currentPlayerStats.knockbackLength);
                }

                if (damageText != null && target.tag == "Enemy")
                {
                    Instantiate(damageText, target.transform.position, Quaternion.identity);
                    damageText.SetText(currentPlayerStats.attackDamage);
                }
                damageable.Damage(currentPlayerStats.attackDamage, false);
            }
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
    public void GainBaseballBat()
    {
        hasBaseballBat = true;
        myAnimator.runtimeAnimatorController = redScarfBaseballbatController;
    }

    private void StopRoll()
    {
        //redScarf.StopRoll();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        if (attackPoint == null)
            return;
        //Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
