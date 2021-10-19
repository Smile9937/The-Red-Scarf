using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour
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
    [SerializeField] protected bool grounded;
    protected float jumpTimeCounter;
    protected bool stoppedJumping;

    //[Header("Attack Variables")]

    [Header("Character Status")]
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;
    [SerializeField] private float secondsOfInvincibility = 0.5f;
    bool isInvincible = false;
    [SerializeField] private GameObject sprite;

    protected Rigidbody2D myRigidbody;
    protected Animator myAnimator;

    public virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        jumpTimeCounter = jumpTime;
        currentHealth = maxHealth;
    }

    public virtual void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, ground);
    }

    public virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected void Move()
    {
        myRigidbody.velocity = new Vector2(direction * speed, myRigidbody.velocity.y);
    }

    protected void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
    }
    protected abstract void HandleJumping();
    protected virtual void HandleMovement()
    {
        Move();
    }
    protected void TurnAround(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    public void TakeDamage(int damage)
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

    IEnumerator InvincibilityFrames()
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
