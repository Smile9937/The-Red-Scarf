using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject bullet;

    [Header("Attack Variables")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 40;

    [SerializeField] private float meleeAttackRate = 2f;
    [SerializeField] private float rangeAttackRate = 1f;
    float nextAttackTime = 0f;
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            if(Input.GetKeyUp("z"))
            {
                MeleeAttack();
                nextAttackTime = Time.time + 1f / meleeAttackRate;
            }

            if(Input.GetKeyUp("x"))
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
