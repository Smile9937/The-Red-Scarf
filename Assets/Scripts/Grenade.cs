using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int explosionDamage;
    [SerializeField] private int timeUntilExplosion;
    [SerializeField] private GameObject explosionOutline;

    private void Start()
    {
        explosionOutline.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            explosionOutline.SetActive(true);
            explosionOutline.transform.localScale = new Vector2(explosionRadius * 4, explosionRadius * 4);
            StartCoroutine(ExplosionTimer());
        }
    }
    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(timeUntilExplosion);
        Explode();
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
            }
        }
        Destroy(gameObject);
    }

}
