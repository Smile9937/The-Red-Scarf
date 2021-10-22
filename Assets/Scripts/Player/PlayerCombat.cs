using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform blockPoint;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private GameObject bullet;

    [Header("Attack Variables")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 40;
    [SerializeField] private float blockHeight = 1f;
    [SerializeField] private float blockWidth = 0.5f;
    [SerializeField] float knockBack = 500f;

    [SerializeField] private float meleeAttackRate = 2f;
    [SerializeField] private float rangeAttackRate = 1f;
    bool blocking = false;
    bool rolling = false;
    float nextMeleeAttackTime = 0f;
    float nextRangedAttackTime = 0f;
    Player player;
    private void Start()
    {
        player = GetComponent<Player>();
    }
    void Update()
    {
        if (!GameManager.Instance.gamePaused)
        {
            if (Time.time >= nextMeleeAttackTime && !blocking)
            {
                if (Input.GetKeyDown("z"))
                {
                    MeleeAttack();
                    nextMeleeAttackTime = Time.time + 1f / meleeAttackRate;
                }
            }

            if (Time.time >= nextRangedAttackTime && !blocking)
            {
                if (Input.GetKey("x"))
                {
                    Shoot();
                    nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
                }
            }

            if (Input.GetKeyDown("q"))
            {
                blocking = true;

                Collider2D[] blockTargets = Physics2D.OverlapCapsuleAll(blockPoint.position, new Vector2(blockHeight, blockWidth), CapsuleDirection2D.Vertical, 0);

                foreach (Collider2D target in blockTargets)
                {
                    Bullet bullet = target.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        Debug.Log("Perfect Block");
                    }
                }
            }

            if(Input.GetKeyUp("q"))
            {
                blocking = false;
            }

            if (blocking)
            {

                Block();
            }

            if(Input.GetKeyDown("e") && !rolling)
            {
                StartCoroutine(Roll());
            }
        }
    }

    private IEnumerator Roll()
    {
        rolling = true;
        player.gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
        yield return new WaitForSeconds(1f);
        rolling = false;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }


    private void Block()
    {
        Collider2D[] blockTargets = Physics2D.OverlapCapsuleAll(blockPoint.position, new Vector2(blockHeight, blockWidth), CapsuleDirection2D.Vertical, 0);

        foreach (Collider2D target in blockTargets)
        {
            Bullet bullet = target.GetComponent<Bullet>();
            if(bullet != null)
            {
                Destroy(bullet.gameObject);
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
