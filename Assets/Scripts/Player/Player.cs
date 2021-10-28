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
        Dead,
    };

    [Serializable]
    public class PlayerStats
    {
        public PlayerCharacterEnum playerCharacter;
        public float speed;
        public float jumpForce;
        public float jumpTime;
    }

    public PlayerStats[] playerCharacters;

    [HideInInspector] public PlayerStats currentCharacter;

    private float speed;
    private float jumpForce;
    private float jumpTime;

    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private float rollSpeedThreshold = 1.5f;
    [SerializeField] private LayerMask rollLayer;
    private float rollSpeed;

    [Header("Block Variables")]
    [SerializeField] private Transform blockPoint;
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private LayerMask blockLayers;

    [Header("Animator Controllers")]
    [SerializeField] private RuntimeAnimatorController redScarfUnarmedAnimator;
    [SerializeField] private RuntimeAnimatorController redScarfBaseballbatController;
    [SerializeField] private RuntimeAnimatorController dressAnimator;

    public bool hasBaseballBat;

    BoxCollider2D myCollider;
    CircleCollider2D rollCollider;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.player = this;
        GameManager.Instance.LoadPlayerStats();
        jumpTimeCounter = jumpTime;
        transform.position = GameManager.Instance.currentSpawnpoint;
        state = State.Neutral;
        myCollider = GetComponent<BoxCollider2D>();
        rollCollider = GetComponent<CircleCollider2D>();
        rollCollider.enabled = false;
        SetAnimator();
        SetCurrentCharacter();
    }

    private void SetAnimator()
    {
        if (hasBaseballBat && GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfBaseballbatController; }
        else if (GameManager.Instance.redScarf) { myAnimator.runtimeAnimatorController = redScarfUnarmedAnimator; }
        else { myAnimator.runtimeAnimatorController = dressAnimator; }
    }
    public void SetCurrentCharacter()
    {
        if (GameManager.Instance.redScarf)
        {
            foreach (PlayerStats character in playerCharacters)
            {
                if (character.playerCharacter == PlayerCharacterEnum.RedScarf)
                {
                    currentCharacter = character;
                }
            }
        }
        else
        {
            foreach (PlayerStats character in playerCharacters)
            {
                if (character.playerCharacter == PlayerCharacterEnum.Dress)
                {
                    currentCharacter = character;
                }
            }
        }

        speed = currentCharacter.speed;
        jumpForce = currentCharacter.jumpForce;
        jumpTime = currentCharacter.jumpTime;
    }

    protected override void Update()
    {
        base.Update();

        myAnimator.SetBool("isGrounded", grounded);

        if (InputManager.Instance.GetKey(KeybindingActions.Left)
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
            SetCurrentCharacter();
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
    private void HandleBlocking()
    {
        if(InputManager.Instance.GetKeyDown(KeybindingActions.Dodge))
        {
            state = State.Blocking;
            myRigidbody.velocity = Vector2.zero;

            Collider2D[] blockTargets = Physics2D.OverlapBoxAll(blockPoint.position, blockSize, 90f, blockLayers);

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
        Collider2D[] blockTargets = Physics2D.OverlapBoxAll(blockPoint.position, blockSize, 90f, blockLayers);

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
            state = State.Rolling;
            rollSpeed = startRollSpeed;
            myRigidbody.velocity = Vector2.zero;
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
            StopRoll();
        }
    }

    private void StopRoll()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1f, rollLayer);
        if (hit.collider == null)
        {
            myAnimator.SetBool("isDodge", false);
            state = State.Neutral;
            gameObject.layer = LayerMask.NameToLayer("Player");
            myCollider.enabled = true;
            rollCollider.enabled = false;
        }
    }
    public void GainBaseballBat()
    {
        hasBaseballBat = true;
        myAnimator.runtimeAnimatorController = redScarfBaseballbatController;
    }

    protected override void Die()
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
    private void OnDrawGizmosSelected()
    {
        if (blockPoint == null)
        return;
        Gizmos.DrawWireCube(blockPoint.position, blockSize);
    }
}
