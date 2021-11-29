using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScarfPlayer : MonoBehaviour
{
    [Header("Roll Variables")]
    [SerializeField] private float startRollSpeed = 200f;
    [SerializeField] private float rollSpeedLoss = 10f;
    [SerializeField] private LayerMask rollLayer;
    private float rollSpeed;

    [Header("Rage Variables")]
    [SerializeField] private int damagePerRage;
    [SerializeField] private float speedPerRage;
    [SerializeField] private int maxRage;
    [SerializeField] private float timeBeforeRageLoss;
    [SerializeField] private int rageLossCount;
    private int currentRageCount;
    private int currentRageDamage;
    private Coroutine loseRageCoroutine;

    [Header("Components")]
    [SerializeField] PlayerStats myStats;
    [SerializeField] private Player player;

    private void Start()
    {
        PlayerUI.Instance.SetSpecialUI(currentRageCount, maxRage);
        loseRageCoroutine = StartCoroutine(LoseRage());
    }
    private void Update()
    {
        if (PauseMenu.Instance.gamePaused)
        {
            return;
        }
        PlayerUI.Instance.SetSpecialUI(currentRageCount, maxRage);
        HandleRage();
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
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Dodge) && player.grounded && !InputManager.Instance.GetKey(KeybindingActions.Attack))
        {
            StartRoll();
        }
    }

    private void StartRoll()
    {
        player.state = Player.State.Rolling;
        rollSpeed = startRollSpeed;
        player.myRigidbody.velocity = Vector2.zero;
        player.myAnimator.SetBool("isDodge", true);
    }

    private void Roll()
    {
        player.gameObject.layer = LayerMask.NameToLayer("Dodge Roll");
        player.rollCollider.enabled = true;
        player.myCollider.enabled = false;
        player.myRigidbody.velocity += new Vector2(Mathf.Sign(transform.rotation.y) * startRollSpeed * Time.deltaTime, 0);

        rollSpeed -= rollSpeed * rollSpeedLoss * Time.deltaTime;
    }


    public void StopRoll()
    {
        player.state = Player.State.ReturningToNeutral;
        player.myRigidbody.velocity = Vector2.zero;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }
    public void ReturnFromRolling()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1f, rollLayer);
        if (hit.collider == null)
        {
            player.state = Player.State.Neutral;
            player.myCollider.enabled = true;
            player.rollCollider.enabled = false;
        }
        if(hit.collider != null)
        {
            StartRoll();
        }
    }
    private void HandleRedScarfMelee()
    {
        if (player.state != Player.State.Neutral) return;
        if (Time.time >= player.nextMeleeAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                player.state = Player.State.Attacking;
                player.myRigidbody.velocity = new Vector2(player.myRigidbody.velocity.x / 2 ,player.myRigidbody.velocity.y);
                player.myAnimator.SetTrigger("attackTrigger");
                //MeleeAttack() called in animation
                player.nextMeleeAttackTime = Time.time + 1f / player.meleeAttackRate;
            }
        }
    }
    public void MeleeAttack()
    {
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(player.attackPoint.position, myStats.attackSize, 90f, player.attackLayers);

        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                ICharacter character = target.GetComponent<ICharacter>();

                if (character != null)
                {
                    character.KnockBack(gameObject, myStats.knockbackVelocity, myStats.knockbackLength);
                    GainRage();
                }

                if (player.damageText != null && target.tag == "Enemy")
                {
                    player.damageText.SetText(myStats.attackDamage + currentRageDamage + player.attackBonus);
                    Instantiate(player.damageText, target.transform.position, Quaternion.identity);
                }
                damageable.Damage(myStats.attackDamage + currentRageDamage + player.attackBonus, false);
            }
        }
    }
    private void GainRage()
    {
        if(loseRageCoroutine != null)
        {
            StopCoroutine(loseRageCoroutine);
        }
        if(currentRageCount + 1 > maxRage)
        {
            currentRageCount = maxRage;
        } else
        {
            currentRageCount++;
        }
        loseRageCoroutine = StartCoroutine(LoseRage());
    }
    private void HandleRage()
    {
        currentRageDamage = currentRageCount * damagePerRage;
        player.speedBonus = currentRageCount * Mathf.Round(speedPerRage * 100) / 100;
    }

    private IEnumerator LoseRage()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeRageLoss);
            if(currentRageCount - rageLossCount <= 0)
            {
                currentRageCount = 0;
            }
            else
            {
                currentRageCount -= rageLossCount;
            }
        }
    }
}
