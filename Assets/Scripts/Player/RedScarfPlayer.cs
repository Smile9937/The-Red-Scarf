using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScarfPlayer : Player
{
    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private float rollSpeedThreshold = 1.5f;
    [SerializeField] private LayerMask rollLayer;
    private float rollSpeed;

    public bool hasBaseballBat;


    protected override void Update()
    {
        base.Update();
        if (!PauseMenu.Instance.gamePaused && state == State.Neutral && hasBaseballBat)
        {
            HandleRedScarfMelee();
        }
    }
    protected override void FixedUpdate()
    {
        if(state == State.Neutral)
        {
            HandleRolling();
        }
        else if(state == State.Rolling)
        {
            Roll();
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

        if (rollSpeed < rollSpeedThreshold)
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
    private void HandleRedScarfMelee()
    {
        if (Time.time >= nextMeleeAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                myAnimator.SetTrigger("attackTrigger");
                //MeleeAttack() called in animation
                nextMeleeAttackTime = Time.time + 1f / meleeAttackRate;
            }
        }
    }
}
