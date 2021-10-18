using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        Jump();
    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
    }

    protected override void HandleJumping()
    {
        
    }

    protected override void Die()
    {
        Debug.Log("Enemy Died");
    }
}
