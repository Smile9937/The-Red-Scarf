using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBoss : TripleBoss
{
    [SerializeField] private Transform topRightLaserPosition;
    [SerializeField] private Transform topLeftLaserPosition;
    [SerializeField] private Transform bottomRightLaserPosition;
    [SerializeField] private Transform bottomLeftLaserPosition;

    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask laserHitLayers;

    [SerializeField] private float timeBeforeLaser;
    [SerializeField] private float timeBeforeEndLaser;
    [SerializeField] private float timeBeforeMoveWithLaser;

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
    protected override void StartCurrentPattern()
    {
        DisableLaser();
        switch(pattern)
        {
            case Pattern.PatternOne:
                MoveToPosition(topLeftLaserPosition);
                break;
            case Pattern.PatternOneMirror:
                MoveToPosition(topRightLaserPosition);
                break;
            case Pattern.PatternTwo:
                MoveToPosition(bottomLeftLaserPosition);
                break;
            case Pattern.PatternTwoMirror:
                MoveToPosition(bottomRightLaserPosition);
                break;
            case Pattern.PatternThree:
                break;
        }
    }

    public override void EndPattern()
    {
        base.EndPattern();
        DisableLaser();
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

    protected override void PositionReached()
    {
        base.PositionReached();
        state = State.Attacking;
        StartCoroutine(WaitToFireLaser());
    }

    protected override void Update()
    {
        base.Update();

        /*var offset = -90f;
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));*/

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
                break;
        }
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
        PatternDone();
    }
}