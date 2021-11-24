using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressPlayer : MonoBehaviour
{
    [Header("Block Variables")]
    [SerializeField] private Transform blockPoint;
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private LayerMask blockLayers;
    [SerializeField] private int blockComposureCost;
    [SerializeField] private float composureLossTimer;
    [SerializeField] private int perfectBlockComposureGain;

    [Header("Composure Variables")]
    public int startComposure;
    [HideInInspector] public int currentComposure;
    [SerializeField] private int composureGain;
    [SerializeField] private float regainTimer;

    private Coroutine loseComposureCoroutine;
    private bool gainComposure;

    [Header("Ranged Attack Variables")]
    [SerializeField] private float rangeAttackRate = 1f;
    [SerializeField] private int gunComposureCost;
    [SerializeField] private float gunJumpForce;
    float nextRangedAttackTime = 0f;

    [SerializeField] private Bullet bullet;

    [SerializeField] private Player player;

    [SerializeField] private PlayerStats myStats;

    private void Start()
    {
        currentComposure = startComposure;
    }
    private void OnEnable()
    {
        StartCoroutine(RegainComposure());
    }
    private void Update()
    {
        if(PauseMenu.Instance.gamePaused)
        {
            return;
        }
        PlayerUI.Instance.SetSpecialUI(currentComposure, 100);
        if (player.state == Player.State.Neutral)
        {
            gainComposure = true;
            HandleBlocking();
            HandleDressRanged();
            HandleDressMelee();
        }
        else if (player.state == Player.State.Blocking)
        {
            gainComposure = false;
            Block();
        }
    }

    private IEnumerator RegainComposure()
    {
        if (gainComposure)
        {
            currentComposure += composureGain;
            if (currentComposure > startComposure)
            {
                currentComposure = startComposure;
            }
        }
        yield return new WaitForSeconds(regainTimer);
        StartCoroutine(RegainComposure());
    }

    private void HandleBlocking()
    {
        if (currentComposure < blockComposureCost)
        {
            Debug.Log("Not enough composure");
            if (loseComposureCoroutine != null)
            {
                StopCoroutine(loseComposureCoroutine);
            }
            player.state = Player.State.Neutral;
        }
        else
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Dodge))
            {
                player.state = Player.State.Blocking;
                loseComposureCoroutine = StartCoroutine(LoseComposure());
                player.myRigidbody.velocity = new Vector2(0, player.myRigidbody.velocity.y);

                Collider2D[] blockTargets = Physics2D.OverlapBoxAll(blockPoint.position, blockSize, 90f, blockLayers);

                foreach (Collider2D target in blockTargets)
                {
                    Bullet bullet = target.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        if (currentComposure + perfectBlockComposureGain <= startComposure)
                        {
                            currentComposure += perfectBlockComposureGain;
                        }
                        else
                        {
                            currentComposure = startComposure;
                        }
                        Debug.Log("Perfect Block");
                    }
                }
            }
        }
    }

    private void Block()
    {
        if (currentComposure < blockComposureCost)
        {
            Debug.Log("Not enough composure");
            StopCoroutine(loseComposureCoroutine);
            player.state = Player.State.Neutral;
        }
        else
        {
            player.myRigidbody.velocity = new Vector2(0, player.myRigidbody.velocity.y);
            if (InputManager.Instance.GetKeyUp(KeybindingActions.Dodge))
            {
                StopCoroutine(loseComposureCoroutine);
                player.state = Player.State.Neutral;
            }
        }
    }
    private IEnumerator LoseComposure()
    {
        while (true)
        {
            currentComposure -= blockComposureCost;
            yield return new WaitForSeconds(composureLossTimer);
        }
    }
    private void HandleDressMelee()
    {
        if (player.state != Player.State.Neutral)
            return;
        if (Time.time >= player.nextMeleeAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                player.state = Player.State.Attacking;
                player.myRigidbody.velocity = new Vector2(player.myRigidbody.velocity.x / 2, player.myRigidbody.velocity.y);
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
                }

                if (player.damageText != null && target.tag == "Enemy")
                {
                    Instantiate(player.damageText, target.transform.position, Quaternion.identity);
                    player.damageText.SetText(myStats.attackDamage + player.attackBonus);
                }
                damageable.Damage(myStats.attackDamage + player.attackBonus, false);
            }
        }
    }
    private void HandleDressRanged()
    {
        if (player.state != Player.State.Neutral)
            return;
        if (Time.time >= nextRangedAttackTime && currentComposure >= gunComposureCost)
        {
            if (InputManager.Instance.GetKey(KeybindingActions.Special))
            {
                player.state = Player.State.Attacking;
                player.myRigidbody.velocity = new Vector2(player.myRigidbody.velocity.x / 2, player.myRigidbody.velocity.y);
                if (InputManager.Instance.GetKey(KeybindingActions.Down))
                {
                    player.myRigidbody.velocity = new Vector2(player.myRigidbody.velocity.x, player.myRigidbody.velocity.y + gunJumpForce);
                    Shoot(new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.Euler(0, 0, -90));
                }
                else if (InputManager.Instance.GetKey(KeybindingActions.Up))
                {
                    Shoot(new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.Euler(0, 0, 90));
                }
                else
                {
                    Shoot(player.attackPoint.position, player.attackPoint.rotation);
                }
            }
        }
    }

    private void Shoot(Vector3 attackPos, Quaternion rotation)
    {
        currentComposure -= gunComposureCost;
        Bullet currentBullet = Instantiate(bullet, attackPos, rotation);
        currentBullet.damageText = player.damageText;
        nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
        player.state = Player.State.Neutral;
    }

    /*private void OnDrawGizmosSelected()
    {
        if (blockPoint == null)
            return;
        Gizmos.DrawWireCube(blockPoint.position, blockSize);
    }*/
}
