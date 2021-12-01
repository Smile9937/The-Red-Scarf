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
    [SerializeField] private AnimationCurve moveWithLaserCurve;
    [SerializeField] private float moveWithLaserSpeed;
    [SerializeField] private LineRenderer laserPrefab;
    [SerializeField] private LineRenderer laserOutlinePrefab;
    [SerializeField] private Transform firePointDown;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private LayerMask laserHitLayers;
    [SerializeField] private int laserDamage;
    [SerializeField] private Vector2 laserKnockback;
    [SerializeField] private float laserKnockBackLength;
    [SerializeField] private float damageRate;

    [Header("Laser Timers")]
    [SerializeField] private float timeBeforeLaser;
    [SerializeField] private float timeBeforeEndTopLaser;
    [SerializeField] private float timeBeforeEndBottomLaser;

    [Header("Charge Attack Variables")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private Transform topLeftCorner;

    [Header("Charge Attack Timers")]
    [SerializeField] private float timeBeforeCharge;
    [SerializeField] private float chargeTimer;
    [SerializeField] private float stuckInGroundTimer;

    [Header("Final Attack Variables")]
    [SerializeField] private LineRenderer bouncingLaserPrefab;
    [SerializeField] private int numberOfLasersForFinalAttack;
    [SerializeField] private float timeUntilSummonLaser;
    [SerializeField] private float timeUntilFinalPatternEnd;

    [Header("Center of Room")]
    [SerializeField] private Vector2 centerOffset;

    private Vector3 centerPosition;

    private int numberOfCharges = 0;

    private bool laserActive;

    private bool canDamage = true;

    private bool move;
    private Transform currentFirePoint;

    private LaserAnimator laserAnimator;

    private Vector2 target;

    private List<LineRenderer> bouncingLasers = new List<LineRenderer>();

    private Vector3 movePosition;
    private Vector3 laserTarget;
    private Player player;
    private float laserTime;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        laserPrefab = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
        laserOutlinePrefab = Instantiate(laserOutlinePrefab, Vector3.zero, Quaternion.identity);
        laserAnimator = laserPrefab.GetComponent<LaserAnimator>();
        laserOutlinePrefab.enabled = false;
        for(int i = 0; i < numberOfLasersForFinalAttack; i++)
        {
            LineRenderer bouncingLaser = Instantiate(bouncingLaserPrefab, Vector3.zero, Quaternion.identity);
            bouncingLasers.Add(bouncingLaser);
            bouncingLaser.gameObject.SetActive(false);
        }
        DisableLaser();
        List<Vector3> cornerPositions = new List<Vector3>();

        cornerPositions.Add(topRightLaserPosition.transform.position);
        cornerPositions.Add(topLeftLaserPosition.transform.position);
        cornerPositions.Add(bottomRightLaserPosition.transform.position);
        cornerPositions.Add(bottomLeftLaserPosition.transform.position);

        Vector3 movePosition = CenterOfVectors(cornerPositions);
        centerPosition = new Vector3(movePosition.x + centerOffset.x, movePosition.y + centerOffset.y, movePosition.z);

    }
    private Vector3 CenterOfVectors(List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Count == 0)
        {
            return sum;
        }

        foreach (Vector3 vec in vectors)
        {
            sum += vec;
        }
        return sum / vectors.Count;
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
                        CheckIfHit();
                        Move();
                        break;
                    case Pattern.PatternTwo:
                        CheckIfHit();
                        break;
                    case Pattern.PatternTwoMirror:
                        CheckIfHit();
                        break;
                    case Pattern.PatternThree:
                        //transform.Translate(Vector3.left * chargeSpeed * Time.deltaTime);
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
                //RotateToPlayer(180);
                break;
        }
    }

    //Called In Animator
    private void MoveLaser()
    {
        switch(pattern)
        {
            case Pattern.PatternOne:
                movePosition = new Vector2(Mathf.Lerp(bottomRightLaserPosition.position.x, bottomLeftLaserPosition.position.x, 0.5f), transform.position.y);
                break;
            case Pattern.PatternOneMirror:
                movePosition = new Vector2(Mathf.Lerp(bottomRightLaserPosition.position.x, bottomLeftLaserPosition.position.x, 0.5f), transform.position.y);
                break;
        }
        laserTime = 0;
        move = true;
    }
    private void Move()
    {
        if (!move)
            return;

        transform.position = Vector2.Lerp(transform.position, movePosition, moveWithLaserCurve.Evaluate(laserTime));
        laserTime += moveWithLaserSpeed * Time.deltaTime;
        laserPrefab.SetPositions(new Vector3[] { new Vector3(transform.position.x, laserPrefab.GetPosition(0).y), new Vector3(transform.position.x, laserPrefab.GetPosition(1).y) });
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
                MoveToAttackPosition(bottomRightLaserPosition.position);
                break;
            case Pattern.PatternTwoMirror:
                MoveToAttackPosition(bottomLeftLaserPosition.position);
                break;
            case Pattern.PatternThree:

                MoveToAttackPosition(centerPosition);
                /*bool mirror = UnityEngine.Random.Range(0, 2) == 0;

                if(!mirror)
                {
                    MoveToAttackPosition(topRightCorner.position);
                }
                else
                {
                    MoveToAttackPosition(topLeftCorner.position);
                }*/
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
        laserPrefab.SetPositions(positions);
    }
    private void SetLaserOutlinePosition(Transform firePoint, Vector3 target)
    {
        Vector3[] positions = new Vector3[] { firePoint.position, target };
        laserOutlinePrefab.SetPositions(positions);
    }

    private void EnableLaser()
    {
        laserOutlinePrefab.enabled = false;
        laserActive = true;
        laserPrefab.enabled = true;
        laserAnimator.AnimateLine(currentFirePoint.position, laserTarget, 0.3f);
        if(pattern == Pattern.PatternTwo || pattern == Pattern.PatternTwoMirror)
        {
            StartCoroutine(EndLaser(timeBeforeEndBottomLaser));
        }
    }
    private void DisableLaser()
    {
        laserOutlinePrefab.enabled = false;
        laserActive = false;
        laserPrefab.enabled = false;
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
                laserOutlinePrefab.enabled = true;
                currentFirePoint = firePointDown;
                laserTarget = new Vector3(currentFirePoint.position.x, bottomLeftLaserPosition.position.y);
                SetLaserOutlinePosition(currentFirePoint, laserTarget);
                break;
            case Pattern.PatternTwo:
                laserOutlinePrefab.enabled = true;
                currentFirePoint = firePointRight;
                laserTarget = new Vector3(bottomLeftLaserPosition.position.x, currentFirePoint.position.y);
                SetLaserOutlinePosition(currentFirePoint, laserTarget);
                break;
            case Pattern.PatternTwoMirror:
                laserOutlinePrefab.enabled = true;
                currentFirePoint = firePointRight;
                laserTarget = new Vector3(bottomRightLaserPosition.position.x, currentFirePoint.position.y);
                SetLaserOutlinePosition(currentFirePoint, laserTarget);
                break;
            case Pattern.PatternThree:
                PrepareFinalPattern();
                break;
        }

        StartCoroutine(WaitToFireLaser());
    }

    private void PrepareFinalPattern()
    {
        //PlayAnimation("isDive");
        state = State.PreparingToAttack;
        StartCoroutine(FinalPattern());
    }
    private IEnumerator FinalPattern()
    {
        //yield return new WaitForSeconds(timeBeforeCharge);
        //target = player.transform.position;
        state = State.Attacking;
        //numberOfCharges++;
        //yield return new WaitForSeconds(chargeTimer);
        //PrepareToCharge();

        for(int i = 0; i < bouncingLasers.Count; i++)
        {
            yield return new WaitForSeconds(timeUntilSummonLaser);
            bouncingLasers[i].transform.position = Vector3.zero;
            bouncingLasers[i].gameObject.SetActive(true);
            bouncingLasers[i].transform.position = transform.position;
        }

        yield return new WaitForSeconds(timeUntilFinalPatternEnd);

        bool mirror = UnityEngine.Random.Range(0, 2) == 0;

        if(mirror)
        {
            pattern = Pattern.PatternTwoMirror;
        }
        else
        {
            pattern = Pattern.PatternTwo;
        }
        StartCurrentPattern();
        //MoveToAttackPosition();
    }
    /*private LineRenderer BouncingLaser()
    {
        if(bouncingLasers.Count > 0)
        {
            for (int i = 0; i < bouncingLasers.Count; i++)
            {
                if (!bouncingLasers[i].gameObject.activeInHierarchy)
                {
                    return bouncingLasers[i];
                }
            }
        }


        LineRenderer currentLaser = Instantiate(bouncingLaserPrefab);
        currentLaser.gameObject.SetActive(false);
        bouncingLasers.Add(currentLaser);
        return currentLaser;

    }*/

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

        //transform.localScale = new Vector3(0, 0, transform.localScale.z);

        if(startLocalScale.x * Mathf.Sign(vectorToTarget.x) == 1)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
        }
    }

    private void CheckIfHit()
    {
        if (!laserActive)
            return;

        Vector2 direction = laserPrefab.GetPosition(1) - laserPrefab.GetPosition(0);
        RaycastHit2D hit = Physics2D.Raycast(laserPrefab.GetPosition(0), direction.normalized, direction.magnitude, laserHitLayers);

        if (hit.collider == null)
            return;

        if(hit.collider.CompareTag("Ground"))
        {
            Debug.Log(hit.collider.name);
            laserPrefab.SetPosition(1, hit.point);
        }

        if (!canDamage)
            return;

        Player player = hit.collider.GetComponent<Player>();
        if(player != null)
        {
            canDamage = false;
            StartCoroutine(LaserHitCooldown());
            player.Damage(laserDamage, false);
            player.KnockBack(laserPrefab.gameObject, laserKnockback, laserKnockBackLength);
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
                PlayAnimation("isShootRight");
                break;
            case Pattern.PatternTwoMirror:
                PlayAnimation("isShootLeft");
                break;
            case Pattern.PatternThree:
                break;
        }
    }


    private IEnumerator EndLaser(float timer)
    {
        yield return new WaitForSeconds(timer);
        DisableLaser();
        Rotate(0);
        PatternDone();
    }
}