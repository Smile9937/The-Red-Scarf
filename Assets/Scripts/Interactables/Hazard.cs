using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool bypassInvincibility;
    [SerializeField] private float damageRate = 1f;
    bool canDamage = true;
    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null && canDamage)
        {
            canDamage = false;
            damageable.Damage(damage, bypassInvincibility);
            StartCoroutine(damageTimer(damageable));
        }
    }
    private IEnumerator damageTimer(IDamageable damageable)
    {
        yield return new WaitForSeconds(damageRate);
        canDamage = true;
    }
}
