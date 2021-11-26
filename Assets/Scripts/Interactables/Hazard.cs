using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool bypassInvincibility;
    [SerializeField] private float damageRate = 1f;
    [SerializeField] private Vector2 knockback;
    [SerializeField] private float knockbackLength;
    [SerializeField] private bool dealKnockback;
    public class DamageTarget
    {
        public IDamageable damageable;
        public bool canDamage;
    }

    List<DamageTarget> damageTargets = new List<DamageTarget>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnterCollider(collision.collider);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnterCollider(collision);
    }
    private void EnterCollider(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            ICharacter character = collision.gameObject.GetComponent<ICharacter>();
            DamageTarget damageTarget = new DamageTarget();
            damageTarget.damageable = damageable;
            damageTarget.canDamage = true;
            damageTargets.Add(damageTarget);
            if (character != null && dealKnockback)
            {
                character.KnockBack(gameObject, knockback, knockbackLength);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ExitCollider(collision.collider);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ExitCollider(collision);
    }

    private void ExitCollider(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            for (int i = 0; i < damageTargets.Count; i++)
            {
                if (damageable == damageTargets[i].damageable)
                {
                    damageTargets.Remove(damageTargets[i]);
                    i--;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StayInCollider(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        StayInCollider(collision);
    }

    private void StayInCollider(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            for (int i = 0; i < damageTargets.Count; i++)
            {
                if (damageTargets[i] != null)
                {
                    if (damageable == damageTargets[i].damageable && damageTargets[i] != null)
                    {
                        if (damageTargets[i].canDamage == true)
                        {
                            damageTargets[i].canDamage = false;
                            StartCoroutine(damageTimer(damageTargets[i]));
                            damageable.Damage(damage, bypassInvincibility);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator damageTimer(DamageTarget damageable)
    {
        yield return new WaitForSeconds(damageRate);
        damageable.canDamage = true;
    }
}