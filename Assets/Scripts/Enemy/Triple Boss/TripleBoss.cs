using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TripleBoss : MonoBehaviour, IDamageable
{
    [SerializeField] AnimationCurve movementCurve;

    [Header("Basic Stats")]
    [SerializeField] protected int health;

    [SerializeField] protected float moveSpeed;

    [SerializeField] private Vector2 returnTrajectoryOffset = new Vector2(0f, -8f);

    public Transform startPosition;

    protected Vector3 targetPosition;

    protected TripleBossManager bossManager;

    private string currentAnimation;

    protected Animator myAnimator;

    private Hazard hazardComponent;

    [SerializeField] float time;

    protected Vector3 startLocalScale;

    protected Vector3 firstPosition;

    private Vector3[] positions;

    private const string HOVER_RIGHT = "HoverRight";
    protected const string IDLE = "Idle";
    private const string DEAD = "Death";

    //[HideInInspector]
    public State state;
    public enum State
    {
        Waiting,
        MovingToStart,
        MovingToAttackPosition,
        MovingUp,
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
        PlayAnimation(HOVER_RIGHT);
        state = State.MovingToStart;
        time = 0;
        SetFirstPosition();

        SetReturnPoints(startPosition.position);
    }

    private void SetFirstPosition()
    {
        firstPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (PauseMenu.Instance.gamePaused)
            return;

        switch(state)
        {
            case State.MovingToStart:
                ReturnToStart();
                break;
            case State.MovingToAttackPosition:
                if (time < 1f)
                {
                    time += moveSpeed * Time.deltaTime;
                    transform.position = Vector3.Lerp(firstPosition, targetPosition, time);
                }
                else
                {
                    AttackPositionReached();
                }
                break;
            case State.Dead:
                transform.Translate(Vector3.down * Time.deltaTime);
                break;
        }
    }

    private void SetReturnPoints(Vector3 position)
    {
        positions = new Vector3[3];
        positions[0] = transform.position;
        positions[2] = position;

        Vector2 trajectoryOffset = new Vector2();

        if(positions[0].x > positions[2].x)
        {
            trajectoryOffset = new Vector2(-returnTrajectoryOffset.x, returnTrajectoryOffset.y);
        }
        else
        {
            trajectoryOffset = new Vector2(returnTrajectoryOffset.x, returnTrajectoryOffset.y);
        }

        positions[1] = positions[0] + (positions[2] - positions[0]) / 2 + (Vector3)trajectoryOffset;
        state = State.MovingToStart;
    }

    private void ReturnToStart()
    {
        if (time < 1f)
        {
            time += moveSpeed * Time.deltaTime;
            Vector3 m1 = Vector3.Lerp(positions[0], positions[1], time);
            Vector3 m2 = Vector3.Lerp(positions[1], positions[2], time);
            transform.position = Vector3.Lerp(m1, m2, time);
        }
        else
        {
            state = State.Waiting;
            pattern = Pattern.NoPattern;
            PlayAnimation(IDLE);
            bossManager.ReadyToStartBattle(this);
        }
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
        PlayAnimation(HOVER_RIGHT);
        state = State.MovingToAttackPosition;
        time = 0;
        SetFirstPosition();
    }

    protected virtual void AttackPositionReached()
    {
        PlayAnimation(IDLE);
    }
    
    [SerializeField] BossHealthUI theBossHealthUI;

    public void Damage(int damage, bool bypassInvincibility)
    {
        health -= damage;
        SetTheHealthOfBossUI();
        if (health <= 0)
        {
            hazardComponent.enabled = false;
            state = State.Dead;
            bossManager.BossDead(this);
            OnDeath();
            PlayAnimation(DEAD);
            RemoveFromHealthBarUI();
        }
    }

    protected virtual void OnDeath()
    {

    }

    //Called In Death Animation
    private void Die()
    {
        Destroy(gameObject);
    }

    protected void PlayAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation)
            return;

        myAnimator.Play(newAnimation);

        currentAnimation = newAnimation;
    }

    // Beyond Here is Mikael's Code, above is Simon's
    public int GetBossHealth()
    {
        return health;
    }

    public virtual void StartUpBossUI()
    {
        if (theBossHealthUI != null)
        {
            theBossHealthUI.StartUpUIHealth();
        }
    }

    protected virtual void SetTheHealthOfBossUI()
    {
        if (theBossHealthUI != null)
        {
            theBossHealthUI.SetBossHealthBar();
        }
    }
    private void RemoveFromHealthBarUI()
    {
        if (theBossHealthUI != null)
        {
            theBossHealthUI.theBossesUsed.Remove(this);
            theBossHealthUI = null;
        }
    }
}
