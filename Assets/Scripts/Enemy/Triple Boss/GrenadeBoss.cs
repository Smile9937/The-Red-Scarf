using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBoss : TripleBoss
{
    [Header("Grenade Attack Variables")]
    [SerializeField] private Grenade grenade;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float timeBeforeGrenade;
    [SerializeField] private float timeAfterGrenade = 0.5f;
    [SerializeField] private Transform bottomRightCorner;
    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private float timeBetweenGrenades = 0.2f;
    [SerializeField] private Vector2[] grenadeForces = { new Vector2(30, 30), new Vector2(35, 35), new Vector2(40, 40) };

    [Header("Acid Attack Variables")]
    [SerializeField] private float acidLocationStartOffset = 4;
    [SerializeField] private Transform[] acidLocationsLeft;
    [SerializeField] private Transform[] acidLocationsRight;
    [SerializeField] private Vector2 acidSize = new Vector2(1, 3);
    [SerializeField] private float acidMoveSpeed = 0.7f;
    [SerializeField] private float acidAttackMoveSpeed = 0.7f;
    [SerializeField] private Vector2 acidAttackTrajectoryOffset = new Vector2(0f, 1f);
    [SerializeField] private LayerMask acidLayer;
    [SerializeField] private float timeBeforeAcid;

    [SerializeField] private AcidAnimationPlayer acidDropAnimation;

    [Header("Final Attack Variables")]
    [SerializeField] private Vector2[] largeGrenadeForces = { new Vector2(10, 10), new Vector2(15, 15), new Vector2(20, 20) };
    [SerializeField] private float timeBeforeFinalAttack;
    [SerializeField] private float timeBetweenLargeGrenades;
    [SerializeField] private Grenade largeGrenade;
    [SerializeField] private float timeUntilAcid;
    [SerializeField] private AcidSpot[] acidSpots;
    [SerializeField] private float coolDown;

    private Vector3 centerPosition;
    private Vector3[] points;
    private float acidLerpValue = 0;

    List<Grenade> grenades = new List<Grenade>();

    private const string ATTACK1 = "Attack1Right";
    private const string ATTACK2 = "Attack2";
    private const string ATTACK3 = "Attack3";


    private void Start()
    {
        float xPosition = Mathf.Lerp(acidLocationsLeft[0].position.x, acidLocationsRight[0].position.x, 0.5f);
        float yPosition = Mathf.Lerp(acidLocationsLeft[0].position.y, acidLocationsRight[0].position.y, 0.5f);
        centerPosition = new Vector3(xPosition, yPosition + acidLocationStartOffset, transform.position.z);
        acidDropAnimation.gameObject.SetActive(false);
    }
    protected override void StartCurrentPattern()
    {
        switch(pattern)
        {
            case Pattern.PatternOne:
                MoveToAttackPosition(bottomLeftCorner.position);
                break;
            case Pattern.PatternOneMirror:
                MoveToAttackPosition(bottomRightCorner.position);
                break;
            case Pattern.PatternTwo:
                MoveToAttackPosition(centerPosition);
                break;
            case Pattern.PatternTwoMirror:
                MoveToAttackPosition(centerPosition);
                break;
            case Pattern.PatternThree:
                MoveToAttackPosition(centerPosition);
                break;
        }
    }

    protected override void AttackPositionReached()
    {
        base.AttackPositionReached();
        switch (pattern)
        {
            case Pattern.PatternOne:
                transform.localScale = startLocalScale;
                Rotate(0, 0, 0);
                state = State.Attacking;
                StartCoroutine(TimeBeforeGrenade(1));
                break;
            case Pattern.PatternOneMirror:
                transform.localScale = startLocalScale;
                Rotate(0, 180, 0);
                state = State.Attacking;
                StartCoroutine(TimeBeforeGrenade(-1));
                break;
            case Pattern.PatternTwo:
                if (state == State.PreparingToAttack)
                    return;
                Rotate(0, 0, 0);
                transform.localScale = startLocalScale;
                StartCoroutine(TimeBeforeAcid(acidLocationsLeft[0].position));
                break;
            case Pattern.PatternTwoMirror:
                if (state == State.PreparingToAttack)
                    return;
                Rotate(0, 180, 0);
                transform.localScale = startLocalScale;
                StartCoroutine(TimeBeforeAcid(acidLocationsRight[0].position));
                break;
            case Pattern.PatternThree:
                state = State.Attacking;
                StartCoroutine(FinalAttack());
                break;
        }
    }
    protected override void Update()
    {
        base.Update();
        switch(state)
        {
            case State.Attacking:
                switch(pattern)
                {
                    case Pattern.PatternTwo:
                        AcidAttack(acidLocationsLeft[1].position);
                        break;
                    case Pattern.PatternTwoMirror:
                        AcidAttack(acidLocationsRight[1].position);
                        break;
                }
                break;
        }
    }

    private void AcidAttack(Vector3 position)
    {
        if (acidLerpValue < 1f)
        {
            acidLerpValue += acidMoveSpeed * Time.deltaTime;
            Vector3 m1 = Vector3.Lerp(points[0], points[1], acidLerpValue);
            Vector3 m2 = Vector3.Lerp(points[1], points[2], acidLerpValue);
            transform.position = Vector3.Lerp(m1, m2, acidLerpValue);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, position, acidAttackMoveSpeed * Time.deltaTime);
            if(transform.position == position)
            {
                acidLerpValue = 0;
                PatternDone();
            }
            else
            {
                Vector3 acidPosition = new Vector3(transform.position.x, transform.position.y - acidSize.y, transform.position.z);

                Collider2D[] acidHits = Physics2D.OverlapBoxAll(acidPosition, acidSize, 0, acidLayer);

                for(int i = 0; i < acidHits.Length; i++)
                {
                    acidDropAnimation.gameObject.SetActive(true);
                    AcidSpot acidSpot = acidHits[i].GetComponent<AcidSpot>();
                    if(acidSpot != null)
                    {
                        acidSpot.ActivateAcid();
                    }
                }
            }
        }
    }

    private void SetAcidAttackPoints(Vector3 position)
    {
        points = new Vector3[3];
        points[0] = transform.position;
        points[2] = position;
        points[1] = points[0] + (points[2] - points[0]) / 2 + (Vector3) acidAttackTrajectoryOffset;
        state = State.Attacking;
    }

    private void Rotate(float xRotation, float yRotation, float zRotation)
    {
        transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);
    }

    private IEnumerator TimeBeforeGrenade(float forceMultiplier)
    {
        if (state != State.Attacking)
            yield return null;

        yield return new WaitForSeconds(timeBeforeGrenade);
        PlayAnimation(ATTACK1);
        for(int i = 0; i < grenadeForces.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenGrenades);
            Grenade currentGrenade = Instantiate(grenade, firePoint.position, Quaternion.identity);
            grenades.Add(currentGrenade);
            Vector2 force = new Vector2(grenadeForces[i].x * 9.82f * forceMultiplier, grenadeForces[i].y * 9.82f);
            currentGrenade.GetComponent<Rigidbody2D>().AddForce(force);
        }
        state = State.Waiting;
        StopAllCoroutines();
        StartCoroutine(TimeAfterGrenade());
    }
    private IEnumerator TimeBeforeAcid(Vector3 position)
    {
        state = State.PreparingToAttack;
        yield return new WaitForSeconds(timeBeforeAcid);
        PlayAnimation(ATTACK2);
        state = State.Attacking;
        SetAcidAttackPoints(position);
    }
    private IEnumerator TimeAfterGrenade()
    {
        yield return new WaitForSeconds(timeAfterGrenade);
        PatternDone();
    }

    private IEnumerator FinalAttack()
    {
        if (state != State.Attacking)
            yield return null;

        yield return new WaitForSeconds(timeBeforeFinalAttack);
        PlayAnimation(ATTACK3);
        for(int i = 0; i < largeGrenadeForces.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenLargeGrenades);

            Grenade currentGrenade = Instantiate(largeGrenade, transform.position, Quaternion.identity);
            grenades.Add(currentGrenade);
            Vector2 force = new Vector2(largeGrenadeForces[i].x * 9.82f, grenadeForces[i].y * 9.82f);
            currentGrenade.GetComponent<Rigidbody2D>().AddForce(force);

            Grenade currentGrenade2 = Instantiate(largeGrenade, transform.position, Quaternion.identity);
            grenades.Add(currentGrenade);
            Vector2 negativeForce = new Vector2(-largeGrenadeForces[i].x * 9.82f, grenadeForces[i].y * 9.82f);
            currentGrenade2.GetComponent<Rigidbody2D>().AddForce(negativeForce);
        }

        yield return new WaitForSeconds(timeUntilAcid);

        bool mirror = Random.Range(0, 2) == 0;

        if (mirror)
        {
            pattern = Pattern.PatternTwoMirror;
        }
        else
        {
            pattern = Pattern.PatternTwo;
        }
        StartCurrentPattern();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        foreach(Grenade grenade in grenades)
        {
            Destroy(grenade.gameObject);
        }
    }
}
