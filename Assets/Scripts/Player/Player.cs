using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override void Start()
    {
        base.Start();
        transform.position = GameManager.Instance.currentSpawnpoint;
    }
    public override void Update()
    {
        base.Update();
        direction = Input.GetAxisRaw("Horizontal");
        HandleJumping();
    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
        TurnAround(direction);
    }

    protected override void HandleJumping()
    {
        if (grounded)
        {
            jumpTimeCounter = jumpTime;
        }

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

    protected override void Die()
    {
        Debug.Log("Player Died");
        GameManager.Instance.RespawnPlayer();
    }
}
