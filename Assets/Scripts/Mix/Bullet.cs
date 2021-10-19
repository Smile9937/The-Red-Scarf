using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 50;
    [SerializeField] private int destroyTimer = 10;
    Rigidbody2D myRigidbody;

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
            Enemy enemy = other.GetComponent<Enemy>();

            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            BreakableObject breakableObject = other.GetComponent<BreakableObject>();

            if(breakableObject != null)
            {
                breakableObject.TakeDamage(damage);
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
