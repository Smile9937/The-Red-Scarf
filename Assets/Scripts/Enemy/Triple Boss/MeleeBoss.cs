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
    [SerializeField] private Bullet shockWave;

    [Header("Charge Variables")]
    [SerializeField] private float timeUntilCharge;
    [SerializeField] private int numberOfCharges = 5;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private List<Transform> corners = new List<Transform>();

    [Header("Large Slam Variables")]
    [SerializeField] private Bullet largeShockWave;
    [SerializeField] private float timeUntilLargeSlam;
    [SerializeField] private float stuckInGroundTimer;

    [Header("Center of Room")]
    [SerializeField] private Vector2 centerOffset;

    private List<Transform> cornerList = new List<Transform>();
    private Transform currentPosition;
    private bool chargeCountReached = false;
    private Vector3 centerPosition;
    private Vector3[] points;
    private float swoopLerpValue = 0;
    private float chargeCounter = 0;
    private bool returnToPosition;
    private void Start()
    {
        cornerList = new List<Transform>(corners);

        List<Vector3> cornerPositions = new List<Vector3>();

        for(int i = 0; i < corners.Count; i++)
        {
            cornerPositions.Add(cornerList[i].position);
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
                            transform.Translate(-Vector3.up * slamSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.position = Vector2.MoveTowards(transform.position, currentPosition.position, chargeSpeed * Time.deltaTime);
                            if (transform.position == currentPosition.position)
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
                for(int i = 0; i < 2; i ++)
                {
                    Vector2 position = new Vector2(transform.position.x, transform.position.y);
                    Bullet currentShockWave = Instantiate(shockWave, position, Quaternion.identity);
                    currentShockWave.transform.eulerAngles = Vector3.forward * i * 180;
                }
                returnToPosition = true;
            }
            else if(pattern == Pattern.PatternThree)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = new Vector2(transform.position.x, transform.position.y + 0.5f);
                    Bullet currentShockWave = Instantiate(largeShockWave, position, Quaternion.identity);
                    currentShockWave.transform.eulerAngles = Vector3.forward * i * 180;
                }
                chargeCounter = 0;
                chargeCountReached = false;
                state = State.Cooldown;
                StartCoroutine(StuckInGroundTimer());
            }
        }

    }

    private IEnumerator StuckInGroundTimer()
    {
        yield return new WaitForSeconds(stuckInGroundTimer);
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
        SetSwoopPoints(position);
    }

    private IEnumerator TimeUntilStartSlam()
    {
        yield return new WaitForSeconds(timeUntilStartSlam);
        returnToPosition = false;
        state = State.Attacking;
    }

    private IEnumerator TimeUntilCharge()
    {
        yield return new WaitForSeconds(timeUntilCharge);
        cornerList = new List<Transform>(corners);
        chargeCounter++;
        int randomNum = Random.Range(0, cornerList.Count);
        currentPosition = cornerList[randomNum];
        cornerList.Remove(currentPosition);
        state = State.Attacking;
    }

    private IEnumerator TimeUntilStartLargeSlam()
    {
        yield return new WaitForSeconds(timeUntilLargeSlam);
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
            cornerList.Add(currentPosition);
            int randomNum = Random.Range(0, cornerList.Count);
            currentPosition = cornerList[randomNum];
            cornerList.Remove(currentPosition);
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
