using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;

    Vector2 startPosition;

    float startZ;

    Vector2 Travel => (Vector2)cam.transform.position - startPosition;

    float DistanceFromSubject => transform.position.z - subject.position.z;

    float ClippingPlane => (cam.transform.position.z + (DistanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane));

    float ParallaxFactor => Mathf.Abs(DistanceFromSubject) / ClippingPlane;

    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void Update()
    {
        Vector2 newPos = startPosition + Travel * ParallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}