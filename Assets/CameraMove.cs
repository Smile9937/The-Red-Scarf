using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float startSpeed = 10;
    [SerializeField] private float startOrtographicSize = 10;
    [SerializeField] private float zoomSpeed = 0.2f;
    [SerializeField] private float speedChangeRate = 0.2f;
    [SerializeField] private float speed;
    [SerializeField] private float ortographicSize;
    [SerializeField] private GameObject[] objectsToDisable;
    private Camera cam;

    private float moveX = 0;
    private float moveY = 0;

    private float zoomLerpDuration = 1f;
    private float timeToResetZoom;

    private bool resetZoom;

    private float speedLerpDuration = 1f;
    private float timeToResetSpeed;

    private bool resetSpeed;

    private bool disableMove;
    private void Start()
    {
        foreach(GameObject gameObject in objectsToDisable)
        {
            gameObject.SetActive(false);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, -15);
        cam = GetComponent<Camera>();
        ortographicSize = startOrtographicSize;
        speed = startSpeed;
    }
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.P))
        {
            disableMove = !disableMove;
        }

        if (disableMove)
            return;

        ChangeMoveSpeed();
        Zoom();
        Move();
        ResetZoom();
        ResetSpeed();
    }
    private void ChangeMoveSpeed()
    {
        if(Input.GetKey(KeyCode.S))
        {
            speed += speedChangeRate;
        }
        if(Input.GetKey(KeyCode.A) && speed > 0)
        {
            speed -= speedChangeRate;
        }
    }
    private void Zoom()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            ortographicSize += zoomSpeed;
        }

        if (Input.GetKey(KeyCode.X) && ortographicSize > 1)
        {
            ortographicSize -= zoomSpeed;
        }

        cam.orthographicSize = ortographicSize;
    }
    private void Move()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1;
        }
        else
        {
            moveX = 0;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveY = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveY = -1;
        }
        else
        {
            moveY = 0;
        }

        transform.position += new Vector3(moveX * speed * Time.deltaTime, moveY * speed * Time.deltaTime, 0);
    }
    private void ResetZoom()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !resetZoom)
        {
            timeToResetZoom = 0;
            resetZoom = true;
        }

        if (!resetZoom)
            return;

        if (timeToResetZoom < zoomLerpDuration)
        {
            float t = timeToResetZoom / zoomLerpDuration;
            t = t * t * (3f - 2f * t);

            ortographicSize = Mathf.Lerp(ortographicSize, startOrtographicSize, t);

            timeToResetZoom += Time.deltaTime;
        }
        else
        {
            ortographicSize = startOrtographicSize;
            resetZoom = false;
        }
    }
    private void ResetSpeed()
    {
        if (Input.GetKeyDown(KeyCode.W) && !resetSpeed)
        {
            timeToResetSpeed = 0;
            resetSpeed = true;
        }

        if (!resetSpeed)
            return;

        if (timeToResetSpeed < speedLerpDuration)
        {
            float t = timeToResetSpeed / speedLerpDuration;
            t = t * t * (3f - 2f * t);

            speed = Mathf.Lerp(speed, startSpeed, t);

            timeToResetSpeed += Time.deltaTime;
        }
        else
        {
            ortographicSize = startOrtographicSize;
            resetSpeed = false;
        }
    }
}
