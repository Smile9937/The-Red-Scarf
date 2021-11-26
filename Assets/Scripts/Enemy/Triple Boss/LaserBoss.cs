using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBoss : TripleBoss
{
    [Header("Laser Positions")]
    [SerializeField] private Transform topRightLaserPosition;
    [SerializeField] private Transform topLeftLaserPosition;
    [SerializeField] private Transform bottomRightLaserPosition;
    [SerializeField] private Transform bottomLeftLaserPosition;

    [Header("Laser Variables")]
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform firePointDown;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private LayerMask laserHitLayers;
    [SerializeField] private int laserDamage;
    [SerializeField] private Vector2 laserKnockback;
    [SerializeField] private float laserKnockBackLength;
    [SerializeField] private float damageRate;

    [Header("Laser Timers")]
    [SerializeField] private float timeBeforeLaser;
    [SerializeField] private float timeBeforeEndTopLaser;
    [SerializeField] private float timeBeforeEndBottomLaser;
    [SerializeField] private float timeBeforeMoveWithLaser;

    [Header("Charge Attack Variables")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private Transform topLeftCorner;

    [Header("Charge Attack Timers")]
    [SerializeField] private float timeBeforeCharge;
    [SerializeField] private float chargeTimer;
    [SerializeField] private float stuckInGroundTimer;


    private int numberOfCharges = 0;

    Vector3 startLocalScale;

    bool laserActive;

    bool canDamage = true;

    bool move;
    Transform currentFirePoint;

    Vector2 target;

    Vector3 movePosition;
    Vector3 laserTarget;
    Player player;

    private void Start()
    {
        startLocalScale = transform.localScale;
        player = GameManager.Instance.player;
        laser = Instantiate(laser, Vector3.zero, Quaternion.identity);
        DisableLaser();
    }
    protected override void Update()
    {
        base.Update();
        switch (state)
        {
            case State.Attacking:
                switch (pattern)
                {
                    case Pattern.PatternOne:
                    case Pattern.PatternOneMirror:
                        laserTarget = new Vector3(currentFirePoint.position.x, bottomLeftLaserPosition.position.y);
                        SetLaserPosition(currentFirePoint, laserTarget);
                        CheckIfHit();
                        Move();
                        break;
                    case Pattern.PatternTwo:
                        laserTarget = new Vector3(bottomRightLaserPosition.position.x, currentFirePoint.position.y);
                        SetLaserPosition(currentFirePoint, laserTarget);
                        CheckIfHit();
                        break;
                    case Pattern.PatternTwoMirror:
                        laserTarget = new Vector3(bottomLeftLaserPosition.position.x, currentFirePoint.position.y);
                        SetLaserPosition(currentFirePoint, laserTarget);
                        CheckIfHit();
                        break;
                    case Pattern.PatternThree:
                        transform.Translate(Vector3.left * chargeSpeed * Time.deltaTime);
                        //Vector2.MoveTowards(transform.position, target, chargeSpeed * Time.deltaTime);
                        break;
                }
                break;
            case State.MovingToAttackPosition:
                switch(pattern)
                {
                    case Pattern.PatternTwo:
                        //LerpRotation(transform.position, targetPosition, 90);
                        break;
                    case Pattern.PatternTwoMirror:
                        //LerpRotation(transform.position, targetPosition, -90);
                        break;
                    case Pattern.PatternThree:
                        break;
                }
                break;
            case State.PreparingToAttack:
                RotateToPlayer(180);
                break;
        }
    }

    private void Move()
    {
        if (!move)
            return;
        transform.position = Vector2.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
        if (transform.position == movePosition)
        {
            move = false;
            StartCoroutine(EndLaser(timeBeforeEndTopLaser));
        }
    }

    protected override void StartCurrentPattern()
    {
        DisableLaser();
        switch(pattern)
        {
            case Pattern.PatternOne:
                MoveToAttackPosition(topLeftLaserPosition.position);
                break;
            case Pattern.PatternOneMirror:
                MoveToAttackPosition(topRightLaserPosition.position);
                break;
            case Pattern.PatternTwo:
                MoveToAttackPosition(bottomLeftLaserPosition.position);
                break;
            case Pattern.PatternTwoMirror:
                MoveToAttackPosition(bottomRightLaserPosition.position);
                break;
            case Pattern.PatternThree:

                bool mirror = UnityEngine.Random.Range(0, 2) == 0;

                if(!mirror)
                {
                    MoveToAttackPosition(topRightCorner.position);
                }
                else
                {
                    MoveToAttackPosition(topLeftCorner.position);
                }
                break;
        }
    }

    public override void EndPattern()
    {
        base.EndPattern();
        DisableLaser();
        Rotate(0);
    }

    private void SetLaserPosition(Transform firePoint, Vector3 target)
    {
        Vector3[] positions = new Vector3[] { firePoint.position, target };
        laser.SetPositions(positions);
    }

    private void EnableLaser()
    {
        laserActive = true;
        laser.enabled = true;
        if(pattern == Pattern.PatternTwo || pattern == Pattern.PatternTwoMirror)
        {
            StartCoroutine(EndLaser(timeBeforeEndBottomLaser));
        }
    }
    private void DisableLaser()
    {
        laserActive = false;
        laser.enabled = false;
    }

    protected override void AttackPositionReached()
    {
        base.AttackPositionReached();
        state = State.Attacking;
        canDamage = true;

        switch(pattern)
        {
            case Pattern.PatternOne:
            case Pattern.PatternOneMirror:
                currentFirePoint = firePointDown;
                break;
            case Pattern.PatternTwo:
                currentFirePoint = firePointRight;
                break;
            case Pattern.PatternTwoMirror:
                currentFirePoint = firePointLeft;
                break;
            case Pattern.PatternThree:
                PrepareToCharge();
                break;
        }

        StartCoroutine(WaitToFireLaser());
    }

    private void PrepareToCharge()
    {
        PlayAnimation("isDive");
        state = State.PreparingToAttack;
        StartCoroutine(TimeUntilCharge());
    }
    private IEnumerator TimeUntilCharge()
    {
        yield return new WaitForSeconds(timeBeforeCharge);
        target = player.transform.position;
        state = State.Attacking;
        //PlayAnimation("isDive");
        numberOfCharges++;
        yield return new WaitForSeconds(chargeTimer);
        PrepareToCharge();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") && pattern == Pattern.PatternThree)
        {
            Debug.Log("Collision");
            if(numberOfCharges >= 3)
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                transform.position = contactPoint;

                state = State.Cooldown;
                Rotate(0);
                PlayAnimation("isStuck");

                StopAllCoroutines();
                StartCoroutine(StuckInGroundTimer());
            }
            else
            {
                Vector2 normal = collision.GetContact(0).normal;

                if (normal.x != 0f)
                {
                    float rotation = -transform.rotation.eulerAngles.z;
                    Rotate(rotation);
                }
                else if (normal.y != 0f)
                {
                    float rotation = -transform.rotation.eulerAngles.z - 180;
                    Rotate(rotation);
                }
            }
        }
    }

    private IEnumerator StuckInGroundTimer()
    {
        yield return new WaitForSeconds(stuckInGroundTimer);
        numberOfCharges = 0;
        Rotate(0);
        PatternDone();
    }

    private void LerpRotation(Vector3 currentPosition, Vector3 targetPosition, float rotationValue)
    {
        float distance = Vector2.Distance(currentPosition, targetPosition);
        float lerpValue = Mathf.Clamp(1 - (0.1f * distance), 0, 1);
        float rotation = Mathf.Lerp(0, rotationValue, lerpValue);
        Rotate(rotation);
    }
    private void Rotate(float zRotation)
    {
        transform.eulerAngles = Vector3.forward * zRotation;
    }
    private void RotateToPlayer(float offset)
    {
        Vector3 vectorToTarget = player.transform.position - transform.position;
        //transform.localScale = new Vector2(startLocalScale.x * Mathf.Sign(vectorToTarget.x), transform.localScale.y);
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - offset;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);

        if(startLocalScale.x * Mathf.Sign(vectorToTarget.x) == 1)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
        }
    }

    private void CheckIfHit()
    {
        if (!laserActive)
            return;

        Vector2 direction = laserTarget - currentFirePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(currentFirePoint.position, direction.normalized, direction.magnitude, laserHitLayers);

        if (hit.collider == null)
            return;

        if(hit.collider.CompareTag("Ground"))
        {
            Debug.Log(hit.collider.name);
            laser.SetPosition(1, hit.point);
        }

        if (!canDamage)
            return;

        Player player = hit.collider.GetComponent<Player>();
        if(player != null)
        {
            canDamage = false;
            StartCoroutine(LaserHitCooldown());
            player.Damage(laserDamage, false);
            player.KnockBack(laser.gameObject, laserKnockback, laserKnockBackLength);
        }
    }
    private IEnumerator LaserHitCooldown()
    {
        yield return new WaitForSeconds(damageRate);
        canDamage = true;
    }

    private IEnumerator WaitToFireLaser()
    {
        yield return new WaitForSeconds(timeBeforeLaser);
        switch (pattern)
        {
            case Pattern.PatternOne:
            case Pattern.PatternOneMirror:
                PlayAnimation("isShootDown");
                break;
            case Pattern.PatternTwo:
                PlayAnimation("isShootLeft");
                break;
            case Pattern.PatternTwoMirror:
                PlayAnimation("isShootRight");
                break;
            case Pattern.PatternThree:
                break;
        }
    }

    private void MoveLaser()
    {
        //yield return new WaitForSeconds(timeBeforeMoveWithLaser);
        switch(pattern)
        {
            case Pattern.PatternOne:
                movePosition = new Vector2(Mathf.Lerp(bottomRightLaserPosition.position.x, bottomLeftLaserPosition.position.x, 0.5f), transform.position.y);
                break;
            case Pattern.PatternOneMirror:
                movePosition = new Vector2(Mathf.Lerp(bottomRightLaserPosition.position.x, bottomLeftLaserPosition.position.x, 0.5f), transform.position.y);
                break;
        }
        move = true;
    }

    private IEnumerator EndLaser(float timer)
    {
        yield return new WaitForSeconds(timer);
        DisableLaser();
        Rotate(0);
        PatternDone();
    }
}