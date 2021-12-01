using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TripleBoss : MonoBehaviour, IDamageable
{
    [SerializeField] AnimationCurve movementCurve;

    [Header("Basic Stats")]
    [SerializeField] protected int health;

    [SerializeField] protected float moveSpeed;

    public Transform startPosition;

    protected Vector3 targetPosition;

    protected TripleBossManager bossManager;

    public string currentAnimation;

    protected Animator myAnimator;

    private Hazard hazardComponent;

    [SerializeField] float time;

    float speedOffset;

    protected Vector3 startLocalScale;

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
        hazardComponent = GetComponent<Hazard>();
        state = State.Waiting;
        startLocalScale = transform.localScale;
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
        bossManager.SetPositionAvailable(startPosition);
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

        startPosition = bossManager.GetStartPosition(this);

        if (transform.position.x > startPosition.position.x)
        {
            transform.localScale = new Vector3(startLocalScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-startLocalScale.x, transform.localScale.y, transform.localScale.z);
        }
        PlayAnimation("isHoverRight");
        state = State.MovingToStart;
        time = 0;
    }

    protected virtual void Update()
    {
        if (PauseMenu.Instance.gamePaused)
            return;

        switch(state)
        {
            case State.MovingToStart:
                transform.position = Vector2.MoveTowards(transform.position, startPosition.position, movementCurve.Evaluate(time));
                if(transform.position == startPosition.position)
                {
                    state = State.Waiting;
                    pattern = Pattern.NoPattern;
                    PlayAnimation("isIdle");
                    bossManager.ReadyToStartBattle(this);
                }
                break;
            case State.MovingToAttackPosition:
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementCurve.Evaluate(time));
                if(transform.position == targetPosition)
                {
                    AttackPositionReached();
                }
                break;
            case State.Dead:
                transform.Translate(Vector3.down * Time.deltaTime);
                break;
        }
        time += moveSpeed * Time.deltaTime;
    }

    protected void MoveToAttackPosition(Vector3 target)
    {
        targetPosition = target;
        if(transform.position.x > targetPosition.x)
        {
            transform.localScale = new Vector3(startLocalScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-startLocalScale.x, transform.localScale.y, transform.localScale.z);
        }
        PlayAnimation("isHoverRight");
        state = State.MovingToAttackPosition;
        time = 0;
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
            hazardComponent.enabled = false;
            state = State.Dead;
            bossManager.BossDead(this);
            PlayAnimation("isDead");
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

        foreach (AnimatorControllerParameter parameter in myAnimator.parameters)
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
