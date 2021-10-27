using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [HideInInspector]
    public State state;
    public enum State
    {
        Neutral,
        Rolling,
        Blocking,
        Dash,
    };

    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private float rollSpeedThreshold = 1.5f;
    [SerializeField] private LayerMask rollLayer;
    private float rollSpeed;

    [Header("Block Variables")]
    [SerializeField] private Transform blockPoint;
    [SerializeField] private float blockHeight = 1f;
    [SerializeField] private float blockWidth = 0.5f;

    [Header("Animator Controllers")]
    [SerializeField] private RuntimeAnimatorController redScarfUnarmedAnimator;
    [SerializeField] private RuntimeAnimatorController redScarfBaseballbatController;
    [SerializeField] private RuntimeAnimatorController dressAnimator;

    [HideInInspector] public bool hasBaseballBat;

    BoxCollider2D myCollider;
    CircleCollider2D rollCollider;
    protected override void Start()
    {
        base.Start();
        transform.position = GameManager.Instance.currentSpawnpoint;
        state = State.Neutral;
        myCollider = GetComponent<BoxCollider2D>();
        rollCollider = GetComponent<CircleCollider2D>();
        rollCollider.enabled = false;
        SetAnimator();
    }

    private void SetAnimator()
    {
        if (hasBaseballBat && GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfBaseballbatController; }
        else if (GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfUnarmedAnimator; }
        else { myAnimator.runtimeAnimatorController = dressAnimator; }
    }

    protected override void Update()
    {
        base.Update();
        
        if(InputManager.Instance.GetKey(KeybindingActions.Left)
            && !InputManager.Instance.GetKey(KeybindingActions.Right))
        {
            direction = -1;
        }
        else if(Input.GetKey(KeyCode.RightArrow) &&
            !InputManager.Instance.GetKey(KeybindingActions.Left))
        {
            direction = 1;
        }
        else
        {
            direction = 0;
        }

        switch(state)
        {
            case State.Neutral:
                HandleJumping();
                if (GameManager.Instance.redScarf)
                {
                    HandleRolling();
                }else
                {
                    HandleBlocking();
                }
                break;
            case State.Blocking:
                Block();
                break;
            case State.Dash:
                Block();
                break;
        }

        //Swap Character
        if(InputManager.Instance.GetKeyDown(KeybindingActions.SwapCharacter))
        {
            GameManager.Instance.SwapCharacter();
            SetAnimator();
        }
    }
    private void FixedUpdate()
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
    private void HandleMovement()
    {
        myAnimator.SetFloat("axisXSpeed", Mathf.Abs(direction));
        TurnAround(direction);
        Move();
    }

    protected void Move()
    {
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

        if (InputManager.Instance.GetKeyDown(KeybindingActions.Jump) && canJump)
        {
            Jump();
            stoppedJumping = false;
        }

        if (InputManager.Instance.GetKey(KeybindingActions.Jump) && !stoppedJumping && jumpTimeCounter > 0)
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
    private void HandleBlocking()
    {
        if(InputManager.Instance.GetKeyDown(KeybindingActions.Dodge))
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

        if(InputManager.Instance.GetKeyUp(KeybindingActions.Dodge))
        {
            state = State.Neutral;
        }
    }
    private void HandleRolling()
    {
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Dodge) && grounded)
        {
            rollSpeed = startRollSpeed;
            myRigidbody.velocity = Vector2.zero;
            state = State.Rolling;
            myAnimator.SetBool("isDodge", true);
        }
    }
    private void Roll()
    {
        gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
        rollCollider.enabled = true;
        myCollider.enabled = false;
        myRigidbody.velocity += new Vector2(Mathf.Sign(transform.rotation.y) * startRollSpeed * Time.deltaTime, 0);

        rollSpeed -= rollSpeed * rollSpeedLoss * Time.deltaTime;

        if(rollSpeed < rollSpeedThreshold)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1f, rollLayer);
            if (hit.collider == null && grounded)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                myCollider.enabled = true;
                rollCollider.enabled = false;
                state = State.Neutral;
                myAnimator.SetBool("isDodge", false);
            }
        }
    }

    public void GainBaseballBat()
    {
        hasBaseballBat = true;
        myAnimator.runtimeAnimatorController = redScarfBaseballbatController;
    }

    protected override void Die()
    {
        Debug.Log("Player Died");
        GameManager.Instance.RespawnPlayer();
    }
}
