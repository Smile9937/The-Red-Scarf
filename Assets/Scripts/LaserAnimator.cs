using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LaserAnimator : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private Texture[] textures;
    [SerializeField] private float fps = 60f;

    [SerializeField] private float animationSpeed;

    private float fpsCounter;

    private int animationStep;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        fpsCounter += Time.deltaTime * animationSpeed;

        if(fpsCounter >= 1f / fps)
        {
            animationStep++;
            if(animationStep == textures.Length)
            {
                animationStep = 0;
            }

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0;
        }
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