using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Movement Variables")]
    [SerializeField] protected float speed = 1.0f;
    protected float direction;

    protected bool facingRight = true;

    [Header("Jump Variables")]
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float jumpTime;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float radius;
    [SerializeField] protected LayerMask ground;
    public bool grounded;
    [SerializeField] protected float offGroundJumpTimer = 0.1f;
    protected float jumpTimeCounter;
    protected bool stoppedJumping = true;

    //[Header("Attack Variables")]

    [Header("Character Status")]
    [SerializeField] protected int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float secondsOfInvincibility;
    bool isInvincible = false;

    protected bool canJump = false;

    private float canJumpCounter;

    protected Rigidbody2D myRigidbody;
    protected Animator myAnimator;

    protected virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        jumpTimeCounter = jumpTime;
        canJumpCounter = offGroundJumpTimer;
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {

        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, ground);

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
    protected void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
    }

    protected abstract void HandleJumping();

    public void Damage(int damage)
    {
        if (isInvincible)
            return;
        currentHealth -= damage;
        Debug.Log("Took " + damage + " damage");

        if(currentHealth <= 0)
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
    protected abstract void Die();
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, radius);
    }
}
