using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 50;
    [SerializeField] private int destroyTimer = 10;
    [SerializeField] private float knockBack = 500;
    Rigidbody2D myRigidbody;

    [Header("Damage Text")]
    [System.NonSerialized] public DamagePopUp damageText;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.velocity = transform.right * speed;
        StartCoroutine(Destroy());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Room")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if(damageable != null)
            {
                if (other.attachedRigidbody != null)
                {
                    Vector2 direction = other.transform.position - transform.position;
                    direction.y = 0;
                    other.attachedRigidbody.AddForce(direction.normalized * knockBack);
                }
                if (damageText != null && other.tag == "Enemy")
                {
                    Instantiate(damageText, transform.position, Quaternion.identity);
                    damageText.SetText(damage);
                }
                damageable.Damage(damage);
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
