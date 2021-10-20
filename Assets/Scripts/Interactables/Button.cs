using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IDamageable
{
    [Tooltip("Only Activatable Objects")]
    [SerializeField] GameObject[] targets;

    [SerializeField] bool isLever = false;
    [SerializeField] float timer;
    bool active = false;
    bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown("q"))
        {
            Damage(0);
        }
    }
    public void Damage(int damage)
    {
        if (isLever)
        {
            active = !active;
            if (active)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
        else
        {
            Activate();
            StartCoroutine(Timer());
        }
    }
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timer);

        Deactivate();
    }
    
    private void Activate()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            IActivatable activatable = targets[i].GetComponent<IActivatable>(); ;
            activatable.Activate();
        }
    }
    private void Deactivate()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            IActivatable activatable = targets[i].GetComponent<IActivatable>(); ;
            activatable.Deactivate();
        }
    }
}
