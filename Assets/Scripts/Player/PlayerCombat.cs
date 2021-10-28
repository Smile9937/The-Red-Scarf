using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Bullet bullet;
    [SerializeField] private DamagePopUp damageText;

    [Header("Attack Variables")]
    [SerializeField] private Vector2 attackSize = new Vector2(0.5f, 5f);
    [SerializeField] private int attackDamage = 40;
    [SerializeField] float knockBack = 500f;

    [SerializeField] private float meleeAttackRate = 2f;
    [SerializeField] private float rangeAttackRate = 1f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;

    [Header("Composure Variables")]
    [SerializeField] private int startComposure;
    [SerializeField] private int gunComposureCost;
    [SerializeField] private int composureGain;
    [SerializeField] private float regainTimer;
    private int currentComposure;

    float nextMeleeAttackTime = 0f;
    float nextRangedAttackTime = 0f;
    Player player;
    Rigidbody2D myRigidbody;
    private void Start()
    {
        currentComposure = startComposure;
        player = GetComponent<Player>();
        myRigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(RegainComposure());
    }
    private IEnumerator RegainComposure()
    {
        currentComposure += composureGain;
        if(currentComposure > startComposure)
        {
            currentComposure = startComposure;
        }
        yield return new WaitForSeconds(regainTimer);
        StartCoroutine(RegainComposure());
    }
    void Update()
    {
        if (PauseMenu.Instance.gamePaused || player.state != Player.State.Neutral)
            return;

        if(GameManager.Instance.redScarf && player.hasBaseballBat)
        {
            if (Time.time >= nextMeleeAttackTime)
            {
                if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
                {
                    player.myAnimator.SetTrigger("attackTrigger");
                    MeleeAttack();
                    nextMeleeAttackTime = Time.time + 1f / meleeAttackRate;
                }
            }
        }
        else if(!GameManager.Instance.redScarf)
        {
            if (Time.time >= nextRangedAttackTime && currentComposure >= gunComposureCost)
            {
                if (Input.GetKey(InputManager.Instance.GetKeyForAction(KeybindingActions.Special)))
                {
                    if(Input.GetKey(InputManager.Instance.GetKeyForAction(KeybindingActions.Down)))
                    {   
                        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                        Shoot(new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.Euler(0, 0, -90));
                    }
                    else if(Input.GetKey(InputManager.Instance.GetKeyForAction(KeybindingActions.Up)))
                    {   
                        Shoot(new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.Euler(0, 0, 90));
                    }
                    else
                    {
                        Shoot(attackPoint.position, attackPoint.rotation);
                    }

                    nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
                }
            }

            if (Time.time >= nextMeleeAttackTime)
            {
                if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
                {
                    player.myAnimator.SetTrigger("attackTrigger");
                    MeleeAttack();
                    nextMeleeAttackTime = Time.time + 1f / meleeAttackRate;
                }
            }
        }
    }

    private void Shoot(Vector3 attackPos, Quaternion rotation)
    {
        currentComposure -= gunComposureCost;
        Bullet currentBullet = Instantiate(bullet, attackPos, rotation);
        currentBullet.damageText = damageText;
    }

    private void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 90f, targetLayers);

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
                if(damageText != null && target.tag == "Enemy")
                {
                    Instantiate(damageText, transform.position, Quaternion.identity);
                    damageText.SetText(attackDamage);                
                }
                damageable.Damage(attackDamage);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
