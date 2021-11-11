using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private Enemy enemyParent;
    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            enemyParent.PlayerDetected(collision);
            gameObject.SetActive(false);
        }
    }
}
