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
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask laserHitLayers;

    [Header("Laser Timers")]
    [SerializeField] private float timeBeforeLaser;
    [SerializeField] private float timeBeforeEndLaser;
    [SerializeField] private float timeBeforeMoveWithLaser;

    [Header("Charge Attack Variables")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float chargeSpeed;

    [Header("Charge Attack Timers")]
    [SerializeField] private float timeBeforeCharge;
    [SerializeField] private float chargeTimer;
    [SerializeField] private float stuckInGroundTimer;


    private int numberOfCharges = 0;

    bool move;
    Vector3 movePosition;
    Vector3 laserTarget;
    Player player;

    private void Start()
    {
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
                        laserTarget = new Vector3(firePoint.position.x, bottomLeftLaserPosition.position.y, 0);
                        SetLaserPosition(laserTarget);
                        CheckIfHitGround();
                        if(move)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
                            if (transform.position == movePosition)
                            {
                                move = false;
                                StartCoroutine(EndLaser());
                            }
                        }
                        break;
                    case Pattern.PatternOneMirror:
                        laserTarget = new Vector3(firePoint.position.x, bottomLeftLaserPosition.position.y, 0);
                        SetLaserPosition(laserTarget);
                        CheckIfHitGround();
                        if (move)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
                            if (transform.position == movePosition)
                            {
                                move = false;
                                StartCoroutine(EndLaser());
                            }
                        }
                        break;
                    case Pattern.PatternTwo:
                        laserTarget = bottomRightLaserPosition.position;
                        SetLaserPosition(laserTarget);
                        CheckIfHitGround();
                        break;
                    case Pattern.PatternTwoMirror:
                        laserTarget = bottomLeftLaserPosition.position;
                        SetLaserPosition(laserTarget);
                        CheckIfHitGround();
                        break;
                    case Pattern.PatternThree:
                        transform.Translate(Vector3.up * chargeSpeed * Time.deltaTime);
                        break;
                }
                break;
            case State.MovingToAttackPosition:
                switch(pattern)
                {
                    case Pattern.PatternTwo:
                        LerpRotation(transform.position, targetPosition, 90);
                        break;
                    case Pattern.PatternTwoMirror:
                        LerpRotation(transform.position, targetPosition, -90);
                        break;
                    case Pattern.PatternThree:
                        break;
                }
                break;
            case State.PreparingToAttack:
                RotateToPlayer(90);
                break;
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
                PrepareToCharge();
                break;
        }
    }

    public override void EndPattern()
    {
        base.EndPattern();
        DisableLaser();
        Rotate(0);
    }

    private void SetLaserPosition(Vector3 target)
    {
        Vector3[] positions = new Vector3[] { firePoint.position, target };
        laser.SetPositions(positions);
    }

    private void EnableLaser()
    {
        laser.enabled = true;
    }
    private void DisableLaser()
    {
        laser.enabled = false;
    }

    protected override void AttackPositionReached()
    {
        base.AttackPositionReached();
        state = State.Attacking;

        switch(pattern)
        {
            case Pattern.PatternTwo:
                break;
            case Pattern.PatternTwoMirror:
                break;
        }

        StartCoroutine(WaitToFireLaser());
    }
    private void PrepareToCharge()
    {
        state = State.PreparingToAttack;
        StartCoroutine(TimeUntilCharge());
    }

    private IEnumerator TimeUntilCharge()
    {
        yield return new WaitForSeconds(timeBeforeCharge);
        state = State.Attacking;
        numberOfCharges++;
        yield return new WaitForSeconds(chargeTimer);
        PrepareToCharge();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") && pattern == Pattern.PatternThree)
        {
            if(numberOfCharges >= 3)
            {
                state = State.Cooldown;
                StopAllCoroutines();
                StartCoroutine(StuckInGroundTimer());
            }
            else
            {
                Vector2 normal = collision.contacts[0].normal;

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
        transform.rotation = Quaternion.Euler(Vector3.forward * zRotation);
    }
    private void RotateToPlayer(float offset)
    {
        Vector3 vectorToTarget = player.transform.position - transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - offset;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
    }

    private void CheckIfHitGround()
    {
        Vector2 direction = laserTarget - firePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction.normalized, direction.magnitude, laserHitLayers);

        if(hit)
        {
            Debug.Log(hit.collider.name);
            laser.SetPosition(1, hit.point);
        }
    }

    private IEnumerator WaitToFireLaser()
    {
        yield return new WaitForSeconds(timeBeforeLaser);
        EnableLaser();
        switch (pattern)
        {
            case Pattern.PatternOne:
            case Pattern.PatternOneMirror:
                StartCoroutine(WaitUntilMove());
                break;
            case Pattern.PatternTwo:
            case Pattern.PatternTwoMirror:
                StartCoroutine(EndLaser());
                break;
            case Pattern.PatternThree:
                break;
        }
    }

    private IEnumerator WaitUntilMove()
    {
        yield return new WaitForSeconds(timeBeforeMoveWithLaser);
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

    private IEnumerator EndLaser()
    {
        yield return new WaitForSeconds(timeBeforeEndLaser);
        DisableLaser();
        Rotate(0);
        PatternDone();
    }
}