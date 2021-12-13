using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 50;
    [SerializeField] private int destroyTimer = 10;

    [SerializeField] private bool bypassInvincibility;

    [Header("Knockback Variables")]
    [SerializeField] private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackLength;

    [Header("Damage Text")]
    [NonSerialized] public DamagePopUp damageText;

    void Start()
    {
        StartCoroutine(Destroy());
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Room")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if(damageable != null)
            {
                ICharacter character = other.GetComponent<ICharacter>();
                if(character != null)
                {
                    character.KnockBack(gameObject, knockbackVelocity, knockbackLength);
                }

                if (damageText != null && other.tag == "Enemy")
                {
                    Instantiate(damageText, transform.position, Quaternion.identity);
                    damageText.SetText(damage, damage);
                }
                damageable.Damage(damage, bypassInvincibility);
            }
            Destroy(gameObject);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(gameObject);
    }
}
