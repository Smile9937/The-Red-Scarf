using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool bypassInvincibility;
    [SerializeField] private float damageRate = 1f;

    public class DamageTarget
    {
        public IDamageable damageable;
        public bool canDamage;
    }

    List<DamageTarget> damageTargets = new List<DamageTarget>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            DamageTarget damageTarget = new DamageTarget();
            damageTarget.damageable = damageable;
            damageTarget.canDamage = true;
            damageTargets.Add(damageTarget);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            for(int i = 0; i < damageTargets.Count; i++)
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
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null)
        {
            for(int i = 0; i < damageTargets.Count; i++)
            {
                if (damageTargets[i] != null)
                {
                    if(damageable == damageTargets[i].damageable && damageTargets[i] != null)
                    {
                        if(damageTargets[i].canDamage == true)
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