using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour
{
    public UnityEvent powerUp;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            powerUp.Invoke();
            Destroy(gameObject);
        }
    }
}
