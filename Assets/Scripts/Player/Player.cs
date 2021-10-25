using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [NonSerialized]
    public State state;
    public enum State
    {
        Neutral,
        Rolling,
        Blocking,
    }

    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private float rollSpeedThreshold = 1.5f;
    private float rollSpeed;

    [Header("Block Variables")]
    [SerializeField] private Transform blockPoint;
    [SerializeField] private float blockHeight = 1f;
    [SerializeField] private float blockWidth = 0.5f;

    Vector2 colliderStartSize;
    BoxCollider2D myCollider;
    private void Awake()
    {
        state = State.Neutral;
    }
    protected override void Start()
    {
        base.Start();
        transform.position = GameManager.Instance.currentSpawnpoint;

        myCollider = GetComponent<BoxCollider2D>();
        colliderStartSize = myCollider.size;
    }
    protected override void Update()
    {
        base.Update();
        direction = Input.GetAxisRaw("Horizontal");

        switch(state)
        {
            case State.Neutral:
                HandleJumping();
                HandleRolling();
                HandleBlocking();
                break;
            case State.Blocking:
                Block();
                break;
        }
    }
    protected override void FixedUpdate()
    {
        switch(state)
        {
            case State.Neutral:
                HandleMovement();
                break;
            case State.Rolling:
                Roll();
                break;
        }

    }
    protected override void HandleMovement()
    {
        base.HandleMovement();
        myAnimator.SetFloat("axisXSpeed", Mathf.Abs(direction));
        TurnAround(direction);
    }

    protected override void HandleJumping()
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
        myAnimator.SetBool("isGrounded", grounded);

        if (Input.GetButtonDown("Jump") && canJump)
        {
            Jump();
            stoppedJumping = false;
        }

        if (Input.GetButton("Jump") && !stoppedJumping && jumpTimeCounter > 0)
        {
            Jump();
            jumpTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
        }
    }
    private void HandleBlocking()
    {
        if(Input.GetKeyDown("q"))
        {
            state = State.Blocking;

            Collider2D[] blockTargets = Physics2D.OverlapCapsuleAll(blockPoint.position, new Vector2(blockHeight, blockWidth), CapsuleDirection2D.Vertical, 0);

            foreach (Collider2D target in blockTargets)
            {
                Bullet bullet = target.GetComponent<Bullet>();
                if (bullet != null)
                {
                    Debug.Log("Perfect Block");
                }
            }
        }
    }
    private void Block()
    {
        Collider2D[] blockTargets = Physics2D.OverlapCapsuleAll(blockPoint.position, new Vector2(blockHeight, blockWidth), CapsuleDirection2D.Vertical, 0);

        foreach (Collider2D target in blockTargets)
        {
            Bullet bullet = target.GetComponent<Bullet>();
            if (bullet != null)
            {
                Destroy(bullet.gameObject);
            }
        }

        if(Input.GetKeyUp("q"))
        {
            state = State.Neutral;
        }
    }
    private void HandleRolling()
    {
        if (Input.GetKeyDown("e") && grounded)
        {
            rollSpeed = startRollSpeed;
            myRigidbody.velocity = Vector2.zero;
            state = State.Rolling;
        }
    }
    private void Roll()
    {
        gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
        myCollider.size = colliderStartSize / 2;
        Debug.Log(myRigidbody.velocity);
        myRigidbody.velocity += new Vector2(Mathf.Sign(transform.rotation.y) * startRollSpeed * Time.deltaTime, 0);

        Debug.Log(Mathf.Sign(transform.rotation.y));

        rollSpeed -= rollSpeed * rollSpeedLoss * Time.deltaTime;

        if(rollSpeed < rollSpeedThreshold)
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            myCollider.size = colliderStartSize;
            state = State.Neutral;
        }
    }

    protected override void Die()
    {
        Debug.Log("Player Died");
        GameManager.Instance.RespawnPlayer();
    }
}
