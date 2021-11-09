using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScarfPlayer : MonoBehaviour
{
    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private float rollSpeedThreshold = 1.5f;
    [SerializeField] private LayerMask rollLayer;
    private float rollSpeed;

    [SerializeField] PlayerStats myStats;

    [SerializeField] private Player player;

    private void Update()
    {
        if(player.state == Player.State.Neutral)
        {
            HandleRolling();
        }

        if (!PauseMenu.Instance.gamePaused && player.state == Player.State.Neutral && player.hasBaseballBat)
        {
            HandleRedScarfMelee();
        }
    }
    private void FixedUpdate()
    {
        if(player.state == Player.State.Rolling)
        {
            Roll();
        }
    }
    private void HandleRolling()
    {
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Dodge) && player.grounded)
        {
            player.state = Player.State.Rolling;
            rollSpeed = startRollSpeed;
            player.myRigidbody.velocity = Vector2.zero;
            player.myAnimator.SetBool("isDodge", true);
        }
    }
    private void Roll()
    {
        gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
        player.rollCollider.enabled = true;
        player.myCollider.enabled = false;
        player.myRigidbody.velocity += new Vector2(Mathf.Sign(transform.rotation.y) * startRollSpeed * Time.deltaTime, 0);

        rollSpeed -= rollSpeed * rollSpeedLoss * Time.deltaTime;

        if (rollSpeed < rollSpeedThreshold)
        {
            StopRoll();
        }
    }

    public void StopRoll()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1f, rollLayer);
        if (hit.collider == null)
        {
            player.myAnimator.SetBool("isDodge", false);
            player.state = Player.State.Neutral;
            gameObject.layer = LayerMask.NameToLayer("Player");
            player.myCollider.enabled = true;
            player.rollCollider.enabled = false;
        }
    }
    private void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(player.attackPoint.position, myStats.attackSize, 90f, player.targetLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                ICharacter character = target.GetComponent<ICharacter>();

                if (character != null)
                {
                    character.KnockBack(gameObject, myStats.knockbackVelocity, myStats.knockbackLength);
                }

                if (player.damageText != null && target.tag == "Enemy")
                {
                    Instantiate(player.damageText, target.transform.position, Quaternion.identity);
                    player.damageText.SetText(myStats.attackDamage);
                }
                damageable.Damage(myStats.attackDamage, false);
            }
        }
    }
    private void HandleRedScarfMelee()
    {
        if (Time.time >= player.nextMeleeAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                player.myAnimator.SetTrigger("attackTrigger");
                //MeleeAttack() called in animation
                player.nextMeleeAttackTime = Time.time + 1f / player.meleeAttackRate;
            }
        }
    }
}
