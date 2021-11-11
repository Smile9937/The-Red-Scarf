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
            if (GetComponent<Animator>())
            {
                GetComponent<Animator>().SetTrigger("pickUpGrabbed");
            }
            else
            {
                powerUp.Invoke();
            }
        }
    }

    private void DestroySelf(float timeUntilDestroyed)
    {
        Destroy(gameObject, timeUntilDestroyed);
    }

    private void InvokePowerUp()
    {
        powerUp.Invoke();
    }
}
