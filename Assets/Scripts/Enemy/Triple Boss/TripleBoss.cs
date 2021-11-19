using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TripleBoss : MonoBehaviour, IDamageable
{
    [SerializeField] protected Transform startPosition;

    [SerializeField] protected int health;

    [SerializeField] protected float moveSpeed;

    protected Transform targetPosition;

    protected TripleBossManager bossManager;

    [HideInInspector]
    public State state;
    public enum State
    {
        Waiting,
        MovingToStart,
        MovingToAttackPosition,
        Attacking
    }

    [HideInInspector]
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
        pattern = Pattern.NoPattern;
    }

    protected void PatternDone()
    {
        MoveToStartPosition();
    }

    private void MoveToStartPosition()
    {
        state = State.MovingToStart;
    }

    protected virtual void Update()
    {
        if (PauseMenu.Instance.gamePaused)
            return;
        switch(state)
        {
            case State.MovingToStart:
                transform.position = Vector2.MoveTowards(transform.position, startPosition.position, moveSpeed * Time.deltaTime);
                if(transform.position == startPosition.position)
                {
                    state = State.Waiting;
                    pattern = Pattern.NoPattern;
                    bossManager.ReadyToStartBattle(this);
                }
                break;
            case State.MovingToAttackPosition:
                transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
                if(transform.position == targetPosition.position)
                {
                    PositionReached();
                }
                break;
        }
    }

    protected void MoveToPosition(Transform target)
    {
        targetPosition = target;
        state = State.MovingToAttackPosition;
    }

    protected virtual void PositionReached()
    {

    }
    public void Damage(int damage, bool bypassInvincibility)
    {
        health -= damage;
        if(health <= 0)
        {
            bossManager.BossDead(this);
            Destroy(gameObject);
        }
    }
}
