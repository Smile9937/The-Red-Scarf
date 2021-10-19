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
            if (isLever)
            {
                active = !active;
                if (active)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].Activate();
                    }
                }
                else
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].Deactivate();
                    }
                }
            }
            else
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].Activate();
                }
                StartCoroutine(Deactivate());
            }
        }
    }
    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(timer);

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].Deactivate();
        }
    }
}
