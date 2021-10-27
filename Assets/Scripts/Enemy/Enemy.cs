using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("Stats")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer;

    [Header("Components")]
    public GameObject hotZone;
    public GameObject triggerArea;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;

    private float distance;
    private bool attackMode;

    private bool cooling;
    private float intTimer;

    protected override void Start()
    {
        base.Start();
        SelectTarget();
        intTimer = timer;
    }

    protected override void Update()
    {
        base.Update();
        myAnimator.SetBool("isGrounded", grounded);
        if(!attackMode)
        {
            MoveAround();
        }

        if(!InsideOfLimits() && !inRange)
        {
            SelectTarget();
        }

        if (inRange)
        {
            EnemyLogic();
        }
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit; 
        }
        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }
        transform.eulerAngles = rotation;
    }

    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);
        if(distance > attackDistance)
        {
            StopAttack();
        }
        else if(attackDistance >= distance && !cooling)
        {
            Attack();
        }
        if(cooling)
        {
            Cooldown();
            //Stop Attack Animation
        }
    }

    private void MoveAround()
    {
        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        myAnimator.SetFloat("axisXSpeed", 1f);
    }

    private void Attack()
    {
        myAnimator.SetTrigger("isAttacking");
        //Start Attack Animation
        timer = intTimer;
        attackMode = true;
        Debug.Log("Attacking");
        TriggerCooling(); //Call In Animator
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        //stop attack animation
    }

    private void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    private void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    protected override void HandleJumping()
    {
        
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }
}
