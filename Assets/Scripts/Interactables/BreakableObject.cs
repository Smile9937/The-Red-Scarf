using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 1;
    [SerializeField] Animator animator = null;

    public void Damage(int damage, bool bypassInvincibility)
    {
        health -= damage;

        if(health <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("startDestroy");
            }
            else
            {
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}