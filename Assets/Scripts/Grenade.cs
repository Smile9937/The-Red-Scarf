using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int explosionDamage;
    [SerializeField] private Vector2 explosionKnockback;
    [SerializeField] private float knockbackLength;
    [SerializeField] private int timeUntilExplosion;
    [SerializeField] private GameObject explosionOutline;

    private Animator myAnimator;
    private Rigidbody2D myRigidbody;

    private void Start()
    {
        explosionOutline.SetActive(false);
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        myAnimator.SetFloat("downWardsVelocity", -myRigidbody.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            myAnimator.SetTrigger("isLanded");
            explosionOutline.SetActive(true);
            explosionOutline.transform.localScale = new Vector2(explosionRadius, explosionRadius);
            StartCoroutine(ExplosionTimer());
        }
    }
    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(timeUntilExplosion);
        myAnimator.SetTrigger("isExploding");
        explosionOutline.SetActive(false);
    }

    private void Explode()
    {
        Collider2D[] explosionHits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, hitLayers);

        for (int i = 0; i < explosionHits.Length; i++)
        {
            Player player = explosionHits[i].GetComponent<Player>();
            if (player != null)
            {
                player.Damage(explosionDamage, false);
                player.KnockBack(gameObject, explosionKnockback, knockbackLength);
            }
        }
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
