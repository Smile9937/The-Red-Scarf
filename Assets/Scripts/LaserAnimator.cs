using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAnimator : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AnimateLine(Vector3 start, Vector3 end, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(Animate(start, end, duration));
    }

    private IEnumerator Animate(Vector3 start, Vector3 end, float duration)
    {
        float startTime = Time.time;
        Vector3 pos = start;
        lineRenderer.SetPosition(0, pos);

        while(pos != end)
        {
            float t = (Time.time - startTime) / duration;
            pos = Vector3.Lerp(start, end, t);
            lineRenderer.SetPosition(1, pos);
            yield return null;
        }
    }
}