using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] Bullet bullet;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(AttackTimer());
    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void HandleMovement()
    {
        base.HandleMovement();
    }

    protected override void HandleJumping()
    {
        
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(bullet, transform.position, Quaternion.identity);
        StartCoroutine(AttackTimer());
    }
}
