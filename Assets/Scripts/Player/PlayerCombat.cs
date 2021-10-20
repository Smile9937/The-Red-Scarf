using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private GameObject bullet;

    [Header("Attack Variables")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 40;

    [SerializeField] float knockBack = 500f;

    [SerializeField] private float meleeAttackRate = 2f;
    [SerializeField] private float rangeAttackRate = 1f;
    float nextAttackTime = 0f;
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            if(Input.GetKeyDown("z"))
            {
                MeleeAttack();
                nextAttackTime = Time.time + 1f / meleeAttackRate;
            }

            if(Input.GetKey("x"))
            {
                Shoot();
                nextAttackTime = Time.time + 1f / rangeAttackRate;
            }
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, attackPoint.position, attackPoint.rotation);
    }

    private void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayers);

        foreach(Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if(damageable != null)
            {
                if(target.attachedRigidbody != null)
                {
                    Vector2 direction = target.transform.position - transform.position;
                    direction.y = 0;
                    target.attachedRigidbody.AddForce(direction.normalized * knockBack);
                }

                damageable.Damage(attackDamage);
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
