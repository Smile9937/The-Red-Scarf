using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public ParticleSystem theLaserStart;
    public ParticleSystem theLaserEnd;
    [SerializeField] float theMaxLengthOfLaser;
    [SerializeField] GameObject theTargetLocation;
    Vector2 theTarget = new Vector2(0, 0);
    [SerializeField] LayerMask theLayerMasks;
    [SerializeField] bool isActivated;

    private LineRenderer theLineRenderer;
    private bool theLaserStartPlaying;
    private bool theLaserEndPlaying;
    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        if (theLineRenderer == null)
            theLineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            if (!theLaserStartPlaying)
            {
                theLaserStartPlaying = true;
                theLaserStart.Play(true);
                theLaserStart.transform.position = transform.position;
            }
            theLineRenderer.enabled = true;

            theTarget = theTargetLocation.transform.position - transform.position;

            hit = Physics2D.Raycast(transform.position, theTarget, theMaxLengthOfLaser, theLayerMasks);
            
            if (hit)
            {
                if (!theLaserEndPlaying)
                {
                    theLaserEndPlaying = true;
                    theLaserEnd.Play();
                }
                theLineRenderer.SetPosition(1, new Vector3(hit.point.x - transform.position.x, hit.point.y - transform.position.y, 0));
                theLaserEnd.gameObject.transform.position = new Vector2(hit.point.x, hit.point.y);
            }
            else
            {
                theLineRenderer.SetPosition(1, new Vector3(Mathf.Clamp(theTarget.x, -theMaxLengthOfLaser, theMaxLengthOfLaser), Mathf.Clamp(theTarget.y, -theMaxLengthOfLaser, theMaxLengthOfLaser), 0));
                if (theLaserEndPlaying)
                {
                    theLaserEndPlaying = false;
                    theLaserEnd.Stop(true);
                }
            }
        }
        else
        {
            if (theLaserStartPlaying)
            {
                theLaserStartPlaying = false;
                theLaserStart.Stop(true);
            }
            if (theLaserEndPlaying)
            {
                theLaserEndPlaying = false;
                theLaserEnd.Stop(true);
            }
        }
    }
}
