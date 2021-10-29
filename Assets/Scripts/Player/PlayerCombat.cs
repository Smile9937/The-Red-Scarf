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
    [SerializeField] private float meleeAttackRate = 2f;
    [SerializeField] private float rangeAttackRate = 1f;

    [Header("Knockback Variables")]
    [SerializeField] private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackLength;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;

    [Header("Composure Variables")]
    [SerializeField] private int startComposure;
    [SerializeField] private int gunComposureCost;
    [SerializeField] private int composureGain;
    [SerializeField] private float regainTimer;
    private int currentComposure;

    [Header("Ground Slam Variables")]
    [SerializeField] private Vector2 groundSlamArea;
    [SerializeField] private LayerMask groundSlamLayer;
    [SerializeField] private int groundSlamDamage;
    [SerializeField] private Vector2 groundSlamKnockbackVelocity;
    [SerializeField] private float groundslamKnockbackLength;
    [SerializeField] private float groundSlamAttackRate = 10f;

    float nextGroundSlamAttackTime = 0f;
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
        HandleGroundSlam();
        if (!PauseMenu.Instance.gamePaused && player.state == Player.State.Neutral)
        {
            if (GameManager.Instance.redScarf && player.hasBaseballBat)
            {
                HandleRedScarfMelee();
            }
            else if (!GameManager.Instance.redScarf)
            {
                HandleDressRanged();
                HandleDressMelee();
            }
        }
    }

    private void HandleGroundSlam()
    {
        if (Time.time >= nextGroundSlamAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack) && InputManager.Instance.GetKey(KeybindingActions.Down) && !player.grounded)
            {
                player.state = Player.State.GroundSlam;
                myRigidbody.velocity = new Vector2(0, -20);
                player.gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
                nextGroundSlamAttackTime = Time.time + 1f / groundSlamAttackRate;
            }
        }

        if (player.state == Player.State.GroundSlam && player.grounded)
        {
            player.state = Player.State.Neutral;
            player.gameObject.layer = LayerMask.NameToLayer("Player");
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(transform.position, groundSlamArea, 0f, groundSlamLayer);

            foreach (Collider2D target in hitTargets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(groundSlamDamage, false);

                    Character character = target.GetComponent<Character>();
                    if (character != null)
                    {
                        character.KnockBack(gameObject, groundSlamKnockbackVelocity, groundslamKnockbackLength);
                        Instantiate(damageText, target.transform.position, Quaternion.identity);
                        damageText.SetText(groundSlamDamage);
                    }
                }
            }
        }
    }

    private void HandleDressMelee()
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
    
    private void HandleDressRanged()
    {
        if (Time.time >= nextRangedAttackTime && currentComposure >= gunComposureCost)
        {
            if (InputManager.Instance.GetKey(KeybindingActions.Special))
            {
                if (InputManager.Instance.GetKey(KeybindingActions.Up))
                {
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                    Shoot(new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.Euler(0, 0, -90));
                }
                else if (InputManager.Instance.GetKey(KeybindingActions.Up))
                {
                    Shoot(new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.Euler(0, 0, 90));
                }
                else
                {
                    Shoot(attackPoint.position, attackPoint.rotation);
                }
            }
        }
    }

    private void HandleRedScarfMelee()
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

    private void Shoot(Vector3 attackPos, Quaternion rotation)
    {
        currentComposure -= gunComposureCost;
        Bullet currentBullet = Instantiate(bullet, attackPos, rotation);
        currentBullet.damageText = damageText;
        nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
    }

    private void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 90f, targetLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Character character = target.GetComponent<Character>();

                if (character != null)
                {
                    character.KnockBack(gameObject, knockbackVelocity, knockbackLength);
                }

                if (damageText != null && target.tag == "Enemy")
                {
                    Instantiate(damageText, target.transform.position, Quaternion.identity);
                    damageText.SetText(attackDamage);
                }
                damageable.Damage(attackDamage, false);
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
