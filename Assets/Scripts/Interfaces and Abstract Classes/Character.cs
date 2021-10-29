using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Movement Variables")]
    protected float direction;

    protected bool facingRight = true;

    [Header("Jump Variables")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float radius;
    [SerializeField] protected Vector2 groundCheckSize;
    [SerializeField] protected LayerMask ground;
    public bool grounded;
    [SerializeField] protected float offGroundJumpTimer = 0.1f;
    protected float jumpTimeCounter;
    protected bool stoppedJumping = true;

    //[Header("Attack Variables")]

    [Header("Character Status")]
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float secondsOfInvincibility;
    bool isInvincible = false;

    protected bool canJump = false;

    private float canJumpCounter;

    protected float knockbackCount;
    private bool knockedFromRight;

    protected Vector2 knockback;

    protected bool canMove;

    protected Rigidbody2D myRigidbody;
    [HideInInspector] public Animator myAnimator;

    protected virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        canJumpCounter = offGroundJumpTimer;
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {

        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, ground);

        if(!grounded && stoppedJumping)
        {
            canJumpCounter -= Time.deltaTime;
        }

        if(grounded)
        {
            canJumpCounter = offGroundJumpTimer;
            canJump = true;
        }

        if(canJumpCounter <= 0)
        {
            canJump = false;
        }

        if(!grounded && !stoppedJumping)
        {
            canJump = false;
        }
    }
    protected virtual void FixedUpdate()
    {
        if(knockbackCount > 0)
        {
            canMove = false;
            if(knockedFromRight)
            {
                myRigidbody.velocity = new Vector2(-knockback.x, knockback.y);
            } else if(!knockedFromRight)
            {
                myRigidbody.velocity = new Vector2(knockback.x, knockback.y);
            }
            knockbackCount -= Time.deltaTime;
        }
    }

    public void Damage(int damage, bool bypassInvincibility)
    {
        if (isInvincible && !bypassInvincibility)
            return;
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(InvincibilityFrames());
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
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        yield return new WaitForSeconds(secondsOfInvincibility);

        isInvincible = false;
    }
    protected abstract void Die();

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
    }
}
