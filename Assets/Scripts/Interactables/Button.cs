using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IDamageable
{
    [Tooltip("Only Activatable Objects")]
    public int id;

    public bool isLever = false;
    public float timer;
    bool playerInRange = false;

    [Header("Save Data")]
    public bool active = false;

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
    private void OnLoad()
    {
        Destroy(gameObject);
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
        GameEvents.current.Activate(id);
    }
    private void Deactivate()
    {
        GameEvents.current.DeActivate(id);
    }
}
