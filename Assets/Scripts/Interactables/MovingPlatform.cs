using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class MovingPlatform : ActivatableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float delayTime;
    [SerializeField] private bool automatic;
    [SerializeField] private bool moveFromEndToStart;
    [SerializeField] private bool moveBackWhenStopped;
    private float tolerance;
    private float delayStart;
    private bool moveBack;
    private bool moveToStart;

    [SerializeField] private List<Vector2> positions;

    private int currentWaypointIndex = 0;
    private Vector3 currentTarget;

    [Header("On Scene Variables")]
    [SerializeField] private List<Vector2> waypointDistances;

    protected override void Start()
    {
        base.Start();
        positions.Insert(0, transform.position);

        currentTarget = transform.position;

        tolerance = speed * Time.deltaTime;
    }
    protected override void Activate()
    {
        automatic = true;
    }

    protected override void DeActivate()
    {
        if(moveBackWhenStopped)
        {
            moveToStart = true;
        }
        else
        {
            automatic = false;
        }
    }

    private void Update()
    {
        if(transform.position != currentTarget)
        {
            MovePlatform();
        }
        else
        {
            UpdateTarget();
        }
        if(moveToStart && (Vector2) transform.position == positions[0])
        {
            moveToStart = false;
            automatic = false;
        }
    }

    private void MovePlatform()
    {
        Vector2 heading = currentTarget - transform.position;
        transform.position += (Vector3) heading / heading.magnitude * speed * Time.deltaTime;
        if(heading.magnitude < tolerance)
        {
            transform.position = currentTarget;
            delayStart = Time.time;
        }
    }
    private void UpdateTarget()
    {
        if(automatic)
        {
            if(Time.time - delayStart > delayTime)
            {
                NextPlatform();
            }
        }

    }
    private void NextPlatform()
    {
        if(!moveBack)
        {
            if(currentWaypointIndex >= positions.Count - 1)
            {
                if(moveFromEndToStart)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    moveBack = true;
                }
            }
            else
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            if(currentWaypointIndex <= 0)
            {
                moveBack = false;
            } else
            {
                currentWaypointIndex--;
            }
        }
        currentTarget = positions[currentWaypointIndex];
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector3 direction = transform.position - other.transform.position;

        if(direction.y < 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            other.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.parent = null;
    }
    private void OnDrawGizmosSelected()
    {
        foreach(Vector2 position in positions)
        {
            Gizmos.DrawCube(position, (Vector2) transform.localScale);
        }
    }

    public void UpdateWaypoints()
    {
        if(positions.Count > 0)
        {
            for(int i = 0; i < positions.Count; i++)
            {
                if(waypointDistances.Count < positions.Count)
                {
                    waypointDistances.Add(Vector3.zero);
                }
            }
        }

        if(waypointDistances.Count > positions.Count)
        {
            int difference = waypointDistances.Count - positions.Count;

            for(int i = 0; i < difference; i++)
            {
                waypointDistances.RemoveAt(waypointDistances.Count - 1);
            }
        }
        
        for(int i = 0; i < positions.Count; i++)
        {
            positions[i] = (Vector2) transform.position + waypointDistances[i];
        }
    }
}
