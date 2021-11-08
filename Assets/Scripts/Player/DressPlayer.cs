using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressPlayer : Player
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

    protected override void Start()
    {
        base.Start();
        currentComposure = startComposure;
        StartCoroutine(RegainComposure());
    }

    protected override void Update()
    {
        base.Update();
        PlayerUI.Instance.SetComposureText(currentComposure);

        if(state == State.Neutral)
        {
            gainComposure = true;
        }
        else if (state == State.Blocking)
        {
            gainComposure = false;
            Block();
        }
        if (!PauseMenu.Instance.gamePaused && state == State.Neutral)
        {
            HandleDressRanged();
            HandleDressMelee();
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
            StopCoroutine(loseComposureCoroutine);
            state = State.Neutral;
        }
        else
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Dodge))
            {
                state = State.Blocking;
                loseComposureCoroutine = StartCoroutine(LoseComposure());
                myRigidbody.velocity = Vector2.zero;

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
            state = State.Neutral;
        }
        else
        {
            Collider2D[] blockTargets = Physics2D.OverlapBoxAll(blockPoint.position, blockSize, 90f, blockLayers);

            foreach (Collider2D target in blockTargets)
            {
                Bullet bullet = target.GetComponent<Bullet>();
                if (bullet != null)
                {
                    Destroy(bullet.gameObject);
                }
            }

            if (InputManager.Instance.GetKeyUp(KeybindingActions.Dodge))
            {
                StopCoroutine(loseComposureCoroutine);
                state = State.Neutral;
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
        if (Time.time >= nextMeleeAttackTime)
        {
            if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                myAnimator.SetTrigger("attackTrigger");
                //MeleeAttack() called in animation
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
                if (InputManager.Instance.GetKey(KeybindingActions.Down))
                {
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, gunJumpForce);
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

    private void Shoot(Vector3 attackPos, Quaternion rotation)
    {
        currentComposure -= gunComposureCost;
        Bullet currentBullet = Instantiate(bullet, attackPos, rotation);
        currentBullet.damageText = damageText;
        nextRangedAttackTime = Time.time + 1f / rangeAttackRate;
    }

    /*private void OnDrawGizmosSelected()
    {
        if (blockPoint == null)
            return;
        Gizmos.DrawWireCube(blockPoint.position, blockSize);
    }*/
}
