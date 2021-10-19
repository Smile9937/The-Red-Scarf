using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public List<GameObject> waypoints;
    public GameObject waypointPrefab;
    public List<GameObject> waypointObjects;

    private int currentWaypointIndex = 0;

    Vector3 currentTarget;

    private void Start()
    {
        if(waypointObjects.Count > 0)
        {
            currentTarget = waypointObjects[currentWaypointIndex].transform.position;
        }
        tolerance = speed * Time.deltaTime;
    }
    public override void Activate()
    {
        automatic = true;
    }

    public override void Deactivate()
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
        if(moveToStart && transform.position == waypointObjects[0].transform.position)
        {
            moveToStart = false;
            automatic = false;
        }
    }

    private void MovePlatform()
    {
        Vector3 heading = currentTarget - transform.position;
        transform.position += heading / heading.magnitude * speed * Time.deltaTime;
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

            if(currentWaypointIndex >= waypointObjects.Count - 1)
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
        currentTarget = waypointObjects[currentWaypointIndex].transform.position;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.parent = null;
    }

    public void GenerateWaypoints()
    {
        if (waypoints.Count > 0)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null)
                {
                    waypoints[i] = waypointPrefab;

                    if (waypointObjects.Count < waypoints.Count)
                    {
                        GameObject currentWaypoint = Instantiate(waypoints[i], transform.position, Quaternion.identity);
                        currentWaypoint.transform.parent = transform.parent;
                        waypointObjects.Add(currentWaypoint);
                    }
                }
            }
        }

        if (waypointObjects.Count > 0)
        {
            for (int j = 0; j < waypointObjects.Count; j++)
            {
                if (j == waypoints.Count)
                {
                    DestroyImmediate(waypointObjects[j]);
                    waypointObjects.RemoveAt(j);
                }
            }
        }
    }
}
