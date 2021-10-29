using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] bool isSingleUse;
    bool hasBeenActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && (!isSingleUse || !hasBeenActivated))
        {
            hasBeenActivated = true;
            GameManager.Instance.SetCurrentCheckpoint(transform.position);
        }
    }
}
