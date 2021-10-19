using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingPlatform : ActivatableObject
{
    [SerializeField] private float tolerance;
    [SerializeField] float speed;
    [SerializeField] float delayTime;
    [SerializeField] bool automatic;
    private float delayStart;

    public List<GameObject> waypoints;
    public GameObject waypointPrefab;
    [HideInInspector] public List<GameObject> waypointObjects;

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

    }

    public override void Deactivate()
    {
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
    }

    private void MovePlatform()
    {
        Vector3 heading = currentTarget - transform.position;
        transform.position += (heading / heading.magnitude) * speed * Time.deltaTime;
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
        currentWaypointIndex++;
        if(currentWaypointIndex >= waypointObjects.Count)
        {
            currentWaypointIndex = 0;
        }
        currentTarget = waypointObjects[currentWaypointIndex].transform.position;
    }
}
