using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy Type")]
    public EnemyType type;
    public enum EnemyType
    {
        Melee,
        Ranged,
    };

    [Header("Stats")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timer;

    [SerializeField] private bool movingEnemy = true;

    [Header("Components")]
    public GameObject hotZone;
    public GameObject triggerArea;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;

    [Header("Melee Attack")]
    [SerializeField] private int meleeDamage;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float meleeRadius = 0.5f;
    [SerializeField] private LayerMask targetLayers;

    [Header("Ranged Attacks")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform rangedAttackPos;
    [SerializeField] private float rangeAttackRate = 1f;
    float nextRangedAttackTime = 0f;

    [SerializeField] private LayerMask blockLayer;

    [Header("Knockback Variables")]

    [SerializeField] protected Vector2 knockbackVelocity;
    [SerializeField] protected float knockbackLength;

    float velocityFactor = -3;

    private float distance;
    private bool attackMode;

    private bool cooling;
    private float intTimer;

    protected override void Start()
    {
        base.Start();
        SelectTarget();
        intTimer = timer;
        if(movingEnemy)
        {
            canMove = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        myAnimator.SetBool("isGrounded", grounded);
        if(!attackMode && movingEnemy && canMove)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.5f, blockLayer);
            if(hit.collider != null)
            {
                inRange = false;
                SelectTarget();
            }
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(knockbackCount <= 0)
        {
            canMove = true;
            if(grounded)
            {
                myRigidbody.velocity = Vector2.zero;
                velocityFactor = -3;
            } else
            {
                velocityFactor -= Time.deltaTime;
                myRigidbody.velocity = new Vector2(0, velocityFactor);
            }
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
            switch(type)
            {
                case EnemyType.Melee:
                    MeleeAttack();
                break;
                case EnemyType.Ranged:
                    if(Time.time >= nextRangedAttackTime)
                    {
                        RangedAttack();
                        nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
                    }
                break;
            }

        }
        if(cooling)
        {
            Cooldown();
            myAnimator.SetBool("isAttackingBool", false);
        }
    }

    private void RangedAttack()
    {
        Instantiate(bullet, rangedAttackPos.position, transform.rotation);
        canMove = false;
        StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(rangeAttackRate);
        canMove = true;
    }
    private void MoveAround()
    {
        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        myAnimator.SetFloat("axisXSpeed", 1f);
    }

    private void MeleeAttack()
    {
        myAnimator.SetBool("isAttackingBool", true);
        myAnimator.SetTrigger("isAttacking");
        timer = intTimer;
        attackMode = true;
        canMove = false;
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
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

    private void DealMeleeDamage()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, meleeRadius, targetLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Character character = target.GetComponent<Character>();

                if (character != null)
                {
                    character.KnockBack(gameObject, knockbackVelocity, knockbackLength);
                }

                damageable.Damage(meleeDamage, false);
            }
        }
    }
    private void TriggerCooling()
    {
        myAnimator.SetBool("isAttackingBool", false);
        cooling = true;
        canMove = true;
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }
    protected override void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, meleeRadius);
    }
}
