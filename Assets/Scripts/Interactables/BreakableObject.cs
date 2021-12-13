using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 1;
    [SerializeField] Animator animator = null;
    private SoundPlayer soundPlayer;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Damage(int damage, bool bypassInvincibility)
    {
        health -= damage;

        if(health <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("startDestroy");
                soundPlayer.PlaySound(1);
            }
            else
            {
                DestroySelf();
                soundPlayer.PlaySound(1);
            }
        } else
        {
            soundPlayer.PlaySound(0);
        }
    }

    private void DestroySelf()
    {
        spriteRenderer.enabled = false;
        StartCoroutine(WaitForSound());
    }
    private IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}