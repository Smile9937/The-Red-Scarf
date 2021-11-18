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
        theLineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            theLineRenderer.enabled = true;

            theTarget = theTargetLocation.transform.position - transform.position;

            hit = Physics2D.Raycast(transform.position, theTarget, theMaxLengthOfLaser, theLayerMasks);
            Debug.DrawLine(transform.position, theTargetLocation.transform.position, new Color(0, 0, 1));
            Debug.DrawRay(transform.position, theTarget, new Color(0,1,0));

            if (hit)
            {
                if (!theLaserEndPlaying)
                {
                    theLaserEndPlaying = true;
                }
                theLaserEnd.Play();
                theLineRenderer.SetPosition(1, new Vector3(hit.point.x - transform.position.x, hit.point.y - transform.position.y, 0));
                theLaserEnd.gameObject.transform.position = new Vector2(hit.point.x * 0.8f, hit.point.y * 0.8f);
            }
            else if (!hit)
            {
                theLineRenderer.SetPosition(1, new Vector3(0, 0, 0));
                theLaserEndPlaying = false;
                theLaserEnd.Stop(true);
            }
        }
        else
        {
            theLaserEndPlaying = false;
            theLaserEnd.Stop(true);
        }
    }
}
