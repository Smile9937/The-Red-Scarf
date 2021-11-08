using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour, IDamageable, ICharacter
{
    [Header("Attack Variables")]
    [SerializeField] private Vector2 attackSize = new Vector2(0.5f, 5f);
    [SerializeField] private int attackDamage = 40;
    [SerializeField] protected float meleeAttackRate = 2f;

    protected float nextGroundSlamAttackTime = 0f;
    protected float nextMeleeAttackTime = 0f;

    [Header("Knockback Variables")]
    [SerializeField] private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackLength;

    [Header("Ground Slam Variables")]
    [SerializeField] private Vector2 groundSlamArea;

    [SerializeField] private int groundSlamDamage;
    [SerializeField] private Vector2 groundSlamKnockbackVelocity;
    [SerializeField] private float groundslamKnockbackLength;
    [SerializeField] private float groundSlamAttackRate = 10f;

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

    [HideInInspector]
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

    public float speed;
    public float jumpForce;
    public float jumpTime;

    [Header("Animator Controllers")]
    [SerializeField] private RuntimeAnimatorController redScarfUnarmedAnimator;
    [SerializeField] protected RuntimeAnimatorController redScarfBaseballbatController;
    [SerializeField] private RuntimeAnimatorController dressAnimator;

    protected BoxCollider2D myCollider;
    protected CircleCollider2D rollCollider;
    protected Rigidbody2D myRigidbody;
    [HideInInspector] public Animator myAnimator;

    [Header("Components")]
    [SerializeField] protected Transform attackPoint;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] protected DamagePopUp damageText;
    [SerializeField] private LayerMask groundSlamLayer;
    protected virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
        rollCollider = GetComponent<CircleCollider2D>();

        canJumpCounter = offGroundJumpTimer;
        currentHealth = maxHealth;

        GameManager.Instance.player = this;
        //GameManager.Instance.LoadPlayerStats();

        jumpTimeCounter = jumpTime;
        transform.position = GameManager.Instance.currentSpawnpoint;
        state = State.Neutral;

        rollCollider.enabled = false;
        SetAnimator();
    }
    private void SetAnimator()
    {
        if (GameManager.Instance.hasBaseballBat && GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfBaseballbatController; }
        else if (GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfUnarmedAnimator; }
        else { myAnimator.runtimeAnimatorController = dressAnimator; }
    }

    protected virtual void Update()
    {
        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, ground);

        HandleGroundSlam();

        CheckIfCanJump();

        PlayerUI.Instance.SetHealthText(currentHealth);
        myAnimator.SetBool("isGrounded", grounded);

        if (InputManager.Instance.GetKey(KeybindingActions.Left)
            && !InputManager.Instance.GetKey(KeybindingActions.Right))
        {
            Debug.Log("Pressed Left");
            direction = -1;
        }
        else if (InputManager.Instance.GetKey(KeybindingActions.Right) &&
            !InputManager.Instance.GetKey(KeybindingActions.Left))
        {
            Debug.Log("Pressed Right");
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
                break;
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

    protected virtual void FixedUpdate()
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

    protected void Move()
    {
        Debug.Log("Move");
        myRigidbody.velocity = new Vector2(direction * speed, myRigidbody.velocity.y);
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
            jumpTimeCounter = jumpTime;
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
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
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

    protected void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 90f, targetLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                ICharacter character = target.GetComponent<ICharacter>();

                if (character != null)
                {
                    character.KnockBack(gameObject, knockbackVelocity, knockbackLength);
                }

                if (damageText != null && target.tag == "Enemy")
                {
                    Instantiate(damageText, target.transform.position, Quaternion.identity);
                    damageText.SetText(attackDamage);
                }
                damageable.Damage(attackDamage, false);
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
                nextGroundSlamAttackTime = Time.time + 1f / groundSlamAttackRate;
            }
        }

        if (state == State.GroundSlam && grounded)
        {
            state = State.Neutral;
            gameObject.layer = LayerMask.NameToLayer("Player");
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(transform.position, groundSlamArea, 0f, groundSlamLayer);

            foreach (Collider2D target in hitTargets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(groundSlamDamage, false);

                    ICharacter character = target.GetComponent<ICharacter>();
                    if (character != null)
                    {
                        character.KnockBack(gameObject, groundSlamKnockbackVelocity, groundslamKnockbackLength);
                        Instantiate(damageText, target.transform.position, Quaternion.identity);
                        damageText.SetText(groundSlamDamage);
                    }
                }
            }
        }
    }
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        if (attackPoint == null)
            return;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }*/
}
