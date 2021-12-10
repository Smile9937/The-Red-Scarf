using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBoss : TripleBoss
{
    [Header("Swoop Variables")]
    [SerializeField] private Transform swoopPositionLeft;
    [SerializeField] private Transform swoopPositionRight;
    [SerializeField] private float swoopSpeed = 0.7f;
    [SerializeField] private Vector2 swoopTrajectoryOffset = new Vector2(0f, -8f);
    [SerializeField] private float timeUntilStartSwoop = 2f;

    [Header("Ground Slam Variables")]
    [SerializeField] private Transform slamPositionLeft;
    [SerializeField] private Transform slamPositionRight;
    [SerializeField] private float slamSpeed;
    [SerializeField] private float timeUntilStartSlam;
    [SerializeField] private Vector2 bossObjectOffset = new Vector2(0, 1);
    [SerializeField] private Bullet shockWave;
    [SerializeField] private GameObject impact;
    [SerializeField] private Vector2 impactOffset = new Vector2(0, 3);
    [SerializeField] private float stuckInGroundTimer;


    [Header("Charge Variables")]
    [SerializeField] private float timeUntilCharge;
    [SerializeField] private int numberOfCharges = 5;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private List<Point> corners = new List<Point>();

    [Header("Large Slam Variables")]
    [SerializeField] private Bullet largeShockWave;
    [SerializeField] private float timeUntilLargeSlam;
    [SerializeField] private float largeWaveStuckInGroundTimer;

    [Header("Center of Room")]
    [SerializeField] private Vector2 centerOffset;

    private Point currentPoint;
    private Point lastPoint;
    private Vector3 currentPosition;
    private bool chargeCountReached = false;
    private Vector3 centerPosition;
    private Vector3[] points;
    private float swoopLerpValue = 0;
    private float chargeCounter = 0;
    private bool returnToPosition;

    private const string DAZED = "Dazed";
    private const string ATTACK1 = "Attack1";
    private const string ATTACK2 = "Attack2";
    private const string CHARGE = "Charge";
    private void Start()
    {
        List<Vector3> cornerPositions = new List<Vector3>();

        for(int i = 0; i < corners.Count; i++)
        {
            cornerPositions.Add(corners[i].transform.position);
        }

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
        switch(state)
        {
            case State.Attacking:

                switch (pattern)
                {
                    case Pattern.PatternOne:
                        SwoopAttack();
                        break;
                    case Pattern.PatternOneMirror:
                        SwoopAttack();
                        break;
                    case Pattern.PatternTwo: case Pattern.PatternTwoMirror:
                        if(!returnToPosition)
                        {
                            transform.Translate(-Vector3.up * slamSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.Translate(Vector3.up * slamSpeed * Time.deltaTime);
                            if(transform.position.y >= startPosition.position.y -2)
                            {
                                PatternDone();
                            }
                        }
                        break;
                    case Pattern.PatternThree:
                        if(chargeCountReached)
                        {
                            soundPlayer.StopSound(2);
                            transform.Translate(-Vector3.up * slamSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.position = Vector2.MoveTowards(transform.position, currentPosition, chargeSpeed * Time.deltaTime);
                            if (transform.position == currentPosition)
                            {
                                PickChargePosition();
                            }
                        }
                        break;
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            if(pattern == Pattern.PatternTwo || pattern == Pattern.PatternTwoMirror)
            {
                soundPlayer.PlaySound(3);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = new Vector2(transform.position.x, transform.position.y + 1);
                    Bullet currentShockWave = Instantiate(shockWave, position, Quaternion.identity);
                    currentShockWave.transform.eulerAngles = Vector3.up * i * 180;

                }
                Vector2 impactPosition = collision.GetContact(0).point + impactOffset;

                GameObject currentImpact = Instantiate(impact, impactPosition, Quaternion.identity);
                Destroy(currentImpact, 0.4f);

                state = State.Cooldown;
                PlayAnimation(DAZED);
                transform.position = collision.GetContact(0).point + bossObjectOffset;
                StartCoroutine(StuckInGroundTimer());
            }
            else if(pattern == Pattern.PatternThree)
            {
                soundPlayer.PlaySound(3);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = new Vector2(transform.position.x, transform.position.y + 1.5f);
                    Bullet currentShockWave = Instantiate(largeShockWave, position, Quaternion.identity);
                    currentShockWave.transform.eulerAngles = Vector3.up * i * 180;

                }
                Vector2 impactPosition = collision.GetContact(0).point + impactOffset + new Vector2(0, 1.3f);

                GameObject currentImpact = Instantiate(impact, impactPosition, Quaternion.identity);
                currentImpact.transform.localScale = new Vector2(currentImpact.transform.localScale.x * 2, currentImpact.transform.localScale.x * 2);
                Destroy(currentImpact, 0.4f);

                PlayAnimation(DAZED);
                chargeCounter = 0;
                chargeCountReached = false;
                state = State.Cooldown;
                transform.position = collision.GetContact(0).point + bossObjectOffset;
                StartCoroutine(LargeWaveStuckInGroundTimer());
            }
        }
    }

    private IEnumerator StuckInGroundTimer()
    {
        yield return new WaitForSeconds(stuckInGroundTimer);
        PlayAnimation(IDLE);
        state = State.Attacking;
        returnToPosition = true;
    }
    private IEnumerator LargeWaveStuckInGroundTimer()
    {
        yield return new WaitForSeconds(largeWaveStuckInGroundTimer);
        PlayAnimation(IDLE);
        state = State.Attacking;
        PickChargePosition();
    }
    private void SwoopAttack()
    {
        if (swoopLerpValue < 1f)
        {
            swoopLerpValue += swoopSpeed * Time.deltaTime;
            Vector3 m1 = Vector3.Lerp(points[0], points[1], swoopLerpValue);
            Vector3 m2 = Vector3.Lerp(points[1], points[2], swoopLerpValue);
            transform.position = Vector3.Lerp(m1, m2, swoopLerpValue);
        }
        else
        {
            soundPlayer.StopSound(2);
            swoopLerpValue = 0;
            PatternDone();
        }
    }

    protected override void AttackPositionReached()
    {
        base.AttackPositionReached();
        state = State.PreparingToAttack;
        switch (pattern)
        {
            case Pattern.PatternOne:
                StartCoroutine(TimeUntilStartSwoop(swoopPositionLeft.position));
                break;
            case Pattern.PatternOneMirror:
                StartCoroutine(TimeUntilStartSwoop(swoopPositionRight.position));
                break;
            case Pattern.PatternTwo:
                StartCoroutine(TimeUntilStartSlam());
                break;
            case Pattern.PatternTwoMirror:
                StartCoroutine(TimeUntilStartSlam());
                break;
            case Pattern.PatternThree:
                if(chargeCountReached)
                {
                    StartCoroutine(TimeUntilStartLargeSlam());
                }
                else
                {
                    StartCoroutine(TimeUntilCharge());
                }
                break;
        }
    }

    private IEnumerator TimeUntilStartSwoop(Vector3 position)
    {
        yield return new WaitForSeconds(timeUntilStartSwoop);
        PlayAnimation(ATTACK2);
        soundPlayer.PlaySound(2);
        SetSwoopPoints(position);
    }

    private IEnumerator TimeUntilStartSlam()
    {
        yield return new WaitForSeconds(timeUntilStartSlam);
        PlayAnimation(ATTACK1);
        returnToPosition = false;
        state = State.Attacking;
    }

    private IEnumerator TimeUntilCharge()
    {
        yield return new WaitForSeconds(timeUntilCharge);
        chargeCounter++;

        int randomNum = Random.Range(0, corners.Count);
        currentPoint = corners[randomNum];
        currentPosition = currentPoint.transform.position;

        PlayAnimation(CHARGE);
        soundPlayer.PlaySound(2);
        state = State.Attacking;
    }

    private IEnumerator TimeUntilStartLargeSlam()
    {
        yield return new WaitForSeconds(timeUntilLargeSlam);
        PlayAnimation(ATTACK1);
        state = State.Attacking;
    }
    private void SetSwoopPoints(Vector3 position)
    {
        points = new Vector3[3];
        points[0] = transform.position;
        points[2] = position;
        points[1] = points[0] + (points[2] - points[0]) / 2 + (Vector3) swoopTrajectoryOffset;
        state = State.Attacking;
    }
    private void PickChargePosition()
    {
        chargeCounter++;
        if (chargeCounter >= numberOfCharges)
        {
            chargeCountReached = true;
            MoveToAttackPosition(centerPosition);
        }
        else
        {
            lastPoint = currentPoint;
            currentPoint = currentPoint.GetPoint(lastPoint);
            currentPosition = currentPoint.transform.position;
            lastPoint = currentPoint;
        }
    }
    protected override void StartCurrentPattern()
    {
        switch (pattern)
        {
            case Pattern.PatternOne:
                MoveToAttackPosition(swoopPositionRight.position);
                break;
            case Pattern.PatternOneMirror:
                MoveToAttackPosition(swoopPositionLeft.position);
                break;
            case Pattern.PatternTwo:
                MoveToAttackPosition(slamPositionRight.position);
                break;
            case Pattern.PatternTwoMirror:
                MoveToAttackPosition(slamPositionLeft.position);
                break;
            case Pattern.PatternThree:
                MoveToAttackPosition(centerPosition);
                break;
        }
    }


}
