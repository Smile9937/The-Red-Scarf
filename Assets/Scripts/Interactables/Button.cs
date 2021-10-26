using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IDamageable
{
    [Tooltip("Only Activatable Objects")]
    public int id;

    public bool isLever = false;
    public float timer;

    public bool active = false;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnLoad()
    {
        Destroy(gameObject);
    }
    public void Damage(int damage)
    {
        if (isLever)
        {
            active = !active;
            animator.SetBool("isActivated", active);
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
            if(timer > 0)
            {
                StartCoroutine(Timer());
            }
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
