using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TripleBoss : MonoBehaviour, IDamageable
{
    [Header("Basic Stats")]
    [SerializeField] protected int health;

    [SerializeField] protected float moveSpeed;

    [SerializeField] protected Transform startPosition;

    protected Vector3 targetPosition;

    protected TripleBossManager bossManager;

    private string currentAnimation;

    protected Animator myAnimator;

    private Collider2D myCollider;

    private Hazard hazardComponent;

    //[HideInInspector]
    public State state;
    public enum State
    {
        Waiting,
        MovingToStart,
        MovingToAttackPosition,
        Cooldown,
        PreparingToAttack,
        Attacking,
        Dead
    }

    //[HideInInspector]
    public Pattern pattern;
    public enum Pattern
    {
        PatternOne,
        PatternOneMirror,
        PatternTwo,
        PatternTwoMirror,
        PatternThree,
        NoPattern
    }
    private void Awake()
    {
        bossManager = GetComponentInParent<TripleBossManager>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
        hazardComponent = GetComponent<Hazard>();
        state = State.Waiting;
    }

    private void OnEnable()
    {
        GameEvents.Instance.onTripleBossMoveToStart += MoveToStartPosition;
        GameEvents.Instance.onTripleBossPatternEnd += EndPattern;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onTripleBossMoveToStart -= MoveToStartPosition;
        GameEvents.Instance.onTripleBossPatternEnd -= EndPattern;
    }

    public void StartPattern(int pattern, bool mirror)
    {
        if(pattern == 1 && !mirror)
        {
            this.pattern = Pattern.PatternOne;
        }
        else if(pattern == 1 && mirror)
        {
            this.pattern = Pattern.PatternOneMirror;
        }
        else if(pattern == 2 && !mirror)
        {
            this.pattern = Pattern.PatternTwo;
        }
        else if(pattern == 2 && mirror)
        {
            this.pattern = Pattern.PatternTwoMirror;
        }
        else if(pattern == 3)
        {
            this.pattern = Pattern.PatternThree;
        }
        StartCurrentPattern();
    }

    protected abstract void StartCurrentPattern();

    public virtual void EndPattern()
    {
        StopAllCoroutines();
        pattern = Pattern.NoPattern;
    }

    protected void PatternDone()
    {
        StopAllCoroutines();
        pattern = Pattern.NoPattern;
        MoveToStartPosition();
    }

    private void MoveToStartPosition()
    {
        if (state == State.Dead)
            return;

        state = State.MovingToStart;
    }

    protected virtual void Update()
    {
        if (PauseMenu.Instance.gamePaused)
            return;
        switch(state)
        {
            case State.MovingToStart:
                PlayAnimation("isHoverRight");
                transform.position = Vector2.MoveTowards(transform.position, startPosition.position, moveSpeed * Time.deltaTime);
                if(transform.position == startPosition.position)
                {
                    state = State.Waiting;
                    pattern = Pattern.NoPattern;
                    PlayAnimation("isIdle");
                    bossManager.ReadyToStartBattle(this);
                }
                break;
            case State.MovingToAttackPosition:
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if(transform.position == targetPosition)
                {
                    AttackPositionReached();
                }
                break;
            case State.Dead:
                transform.Translate(Vector3.down * Time.deltaTime);
                break;
        }
    }

    protected void MoveToAttackPosition(Vector3 target)
    {
        targetPosition = target;
        state = State.MovingToAttackPosition;
    }

    protected virtual void AttackPositionReached()
    {
        PlayAnimation("isIdle");
    }
    public void Damage(int damage, bool bypassInvincibility)
    {
        health -= damage;
        if(health <= 0)
        {
            bossManager.BossDead(this);
            PlayAnimation("isDead");
            state = State.Dead;
            hazardComponent.enabled = false;
        }
    }

    //Called In Death Animation
    private void Die()
    {
        Destroy(gameObject);
    }

    protected void PlayAnimation(string animation)
    {
        if (currentAnimation == animation)
            return;

        foreach(AnimatorControllerParameter parameter in myAnimator.parameters)
        {
            if(parameter.name == currentAnimation)
            {
                myAnimator.SetBool(currentAnimation, false);
                break;
            }
        }
        currentAnimation = animation;
        myAnimator.SetBool(currentAnimation, true);
    }
}
