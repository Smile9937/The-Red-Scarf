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

    [SerializeField] private float jumpForce;
    float nextMeleeAttackTime = 0f;
    float nextRangedAttackTime = 0f;
    Player player;
    Rigidbody2D myRigidbody;
    private void Start()
    {
        player = GetComponent<Player>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (GameManager.Instance.gamePaused || player.state != Player.State.Neutral)
            return;

        if (Time.time >= nextMeleeAttackTime)
        {
            if (Input.GetKeyDown("z"))
            {
                MeleeAttack();
                nextMeleeAttackTime = Time.time + 1f / meleeAttackRate;
            }
        }

        if (Time.time >= nextRangedAttackTime)
        {
            if (Input.GetKey("x"))
            {
                if(Input.GetKey(KeyCode.DownArrow))
                {
                    Shoot(new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.Euler(0, 0, -90));
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                }
                else
                {
                    Shoot(attackPoint.position, attackPoint.rotation);
                }

                nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
            }
        }
    }

    private void Shoot(Vector3 attackPos, Quaternion rotation)
    {
        Instantiate(bullet, attackPos, rotation);
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
