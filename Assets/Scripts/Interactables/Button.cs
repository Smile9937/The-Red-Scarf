using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] ActivatableObject[] targets;
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
            Triggered();
        }
    }
    public void Triggered()
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
            targets[i].Activate();
        }
    }
    private void Deactivate()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].Deactivate();
        }
    }
}
