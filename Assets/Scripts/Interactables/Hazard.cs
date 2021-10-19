using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();

        if(character != null)
        {
            character.TakeDamage(damage);
        }
    }
}
