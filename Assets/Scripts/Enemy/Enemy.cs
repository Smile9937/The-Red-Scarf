using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ICharacter, IGrabbable
{
    [Header("Jump Variables")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask ground;
    public bool grounded;

    [Header("Character Status")]
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float secondsOfInvincibility;
    bool isInvincible = false;

    private float knockbackCount;
    private bool knockedFromRight;

    private Vector2 knockback;

    private Rigidbody2D myRigidbody;
    [HideInInspector] public Animator myAnimator;

    [Header("Enemy Type")]
    public EnemyType type;
    public enum EnemyType
    {
        Melee,
        Ranged,
    };

    [Header("Enemy State")]

    public State state;
    public enum State
    {
        Waiting,
        Stationary,
        Moving,
        ChasePlayer,
        Attacking,
        Staggered,
        Dead
    }

    [Header("Enemy Scarf Interaction")]
    public GrabbingAction scarfActionType;
    public enum GrabbingAction
    {
        Thrown,
        None,
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

     public Transform target;
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

    [Header("Blocking Object")]
    [SerializeField] private LayerMask blockingObjectLayer;

    [Header("Knockback Variables")]

    [SerializeField] private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackLength;

    float velocityFactor = -3;

    private float distance;

    private bool cooling;
    private float intTimer;

    private int idleNoiseCounter = 0;

    private CharacterGrapplingScarf theGrapplingScarf;
    private SoundPlayer soundPlayer;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        currentHealth = maxHealth;

        SelectTarget();
        intTimer = timer;

        theGrapplingScarf = FindObjectOfType<CharacterGrapplingScarf>();

        CheckIfMovingEnemy();
    }

    private void Update()
    {
        if (PauseMenu.Instance.gamePaused)
            return;
        switch(state)
        {
            case State.Moving:
                HandleMoving();
                break;
            case State.ChasePlayer:
                ChasePlayer();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Stationary:
                if(inRange)
                {
                    state = State.Attacking;
                }
                break;
        }

        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, ground);

        myAnimator.SetBool("isGrounded", grounded);
    }
    private void FixedUpdate()
    {
        if (knockbackCount > 0)
        {
            HandleKnockback();
        }
        if (knockbackCount <= 0)
        {
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
    private void HandleMoving()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.5f, blockingObjectLayer);
        if (hit.collider != null)
        {
            inRange = false;
            SelectTarget();
        }
        MoveAround();

        if (!InsideOfLimits() && !inRange)
        {
            SelectTarget();
        }

        if (inRange)
        {
            state = State.ChasePlayer;
        }
    }

    private void HandleAttacking()
    {
        switch (type)
        {
            case EnemyType.Melee:
                MeleeAttack();
                break;
            case EnemyType.Ranged:
                if (Time.time >= nextRangedAttackTime)
                {
                    RangedAttack();
                    nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
                }
                if (!movingEnemy && !inRange)
                {
                    state = State.Stationary;
                }
                else if(!inRange)
                {
                    state = State.Moving;
                }
                break;
        }
    }

    public void PlayerDetected(Collider2D collision)
    {
        if (state == State.Attacking || state == State.Staggered)
            return;
        target = collision.transform;
        inRange = true;
        hotZone.SetActive(true);
        if(movingEnemy)
        {
            state = State.ChasePlayer;
        }
        else if(!movingEnemy)
        {
            state = State.Stationary;
        }
    }

    private void HandleKnockback()
    {
        if (knockedFromRight)
        {
            myRigidbody.velocity = new Vector2(-knockback.x, knockback.y);
        }
        else if (!knockedFromRight)
        {
            myRigidbody.velocity = new Vector2(knockback.x, knockback.y);
        }
        knockbackCount -= Time.deltaTime;
    }

    public void SelectTarget()
    {
        if (state == State.Attacking || state == State.Staggered)
            return;
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
        if (state == State.Attacking || state == State.Staggered)
            return;
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

    private void ChasePlayer()
    {
        distance = Vector2.Distance(transform.position, target.position);
        MoveAround();
        if(!inRange)
        {
            state = State.Moving;
        }
        else if(attackDistance >= distance && !cooling)
        {
            state = State.Attacking;
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
        if(!movingEnemy)
        {
            state = State.Stationary;
        }
    }
    private void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && cooling)
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
                ICharacter character = target.GetComponent<ICharacter>();

                if (character != null)
                {
                    character.KnockBack(gameObject, knockbackVelocity, knockbackLength);
                }

                damageable.Damage(meleeDamage, false);
            }
        }
    }
    private void AttackFinished()
    {
        myAnimator.SetBool("isAttackingBool", false);
        cooling = true;
        CheckIfMovingEnemy();
    }

    private void CheckIfMovingEnemy()
    {
        if (movingEnemy)
        {
            state = State.Moving;
        }
        else
        {
            state = State.Stationary;
        }
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }
    public void Die()
    {
        timer = 0;
        cooling = false;
        inRange = false;
        hotZone.SetActive(false);
        myAnimator.SetBool("isAttackingBool", false);
        myAnimator.Play("Ded");
        //Destroy(gameObject);
        state = State.Dead;
    }

    private void DisableGameObject()
    {
        if (movingEnemy)
        {
            state = State.Moving;
        }
        else
        {
            state = State.Waiting;
        }
        transform.parent.gameObject.SetActive(false);
    }
    public void Damage(int damage, bool bypassInvincibility)
    {
        if (isInvincible && !bypassInvincibility)
            return;
        currentHealth -= damage;
        soundPlayer.PlaySound(0);
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        else // New code
        {
            myAnimator.SetTrigger("isDamaged");
            state = State.Staggered;
        }

        StartCoroutine(InvincibilityFrames());
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        yield return new WaitForSeconds(secondsOfInvincibility);

        isInvincible = false;
    }

    public void KnockBack(GameObject knockbackSource, Vector2 knockbackVelocity, float knockbackLength)
    {
        knockback = knockbackVelocity;
        knockbackCount = knockbackLength;

        knockedFromRight = transform.position.x < knockbackSource.transform.position.x;
    }

    // Scarf Interaction
    public void IsGrabbed()
    {
        switch (scarfActionType)
        {
            case GrabbingAction.Thrown:
                theGrapplingScarf.SetSwingingPointAsTarget(gameObject, -1);
                state = State.Staggered;
                Invoke("ReturnFromGrabbed", 1.5f);
                break;
            case GrabbingAction.None:
                ReturnFromGrabbed();
                break;
            default:
                ReturnFromGrabbed();
                break;
        }
    }
    public void HandleGrabbedTowards()
    {
        switch (scarfActionType)
        {
            case GrabbingAction.Thrown:
                theGrapplingScarf.LaunchPlayerIntoDash();
                CancelInvoke("ReturnFromGrabbed");
                break;
            case GrabbingAction.None:
                break;
            default:
                break;
        }
        ReturnFromGrabbed();
    }
    public void HandleGrabbedAway()
    {
        switch (scarfActionType)
        {
            case GrabbingAction.Thrown:
                KnockBack(theGrapplingScarf.gameObject, new Vector2(1, 5), 0.25f);
                theGrapplingScarf.PlayScarfPullAnimation();
                CancelInvoke("ReturnFromGrabbed");
                break;
            case GrabbingAction.None:
                break;
            default:
                break;
        }
        ReturnFromGrabbed();
    }
    public void ReturnFromGrabbed()
    {
        if (movingEnemy)
        {
            state = State.Moving;
        }
        else
        {
            state = State.Stationary;
        }
        theGrapplingScarf.SetSwingingPointAsTarget(null, 0);
        theGrapplingScarf.ReturnPlayerState();
    }

    private void PlayIdleNoise()
    {
        idleNoiseCounter++;

        if(idleNoiseCounter == 3)
        {
            soundPlayer.PlaySound(1);
            idleNoiseCounter = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, meleeRadius);
    }
}
