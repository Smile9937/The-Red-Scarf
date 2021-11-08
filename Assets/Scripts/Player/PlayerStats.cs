using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats", fileName = "Player Name")]
public class PlayerStats : ScriptableObject
{
    [Header("General Stats")]
    public PlayerCharacterEnum playerCharacter;
    public float speed;
    public float jumpForce;
    public float jumpTime;

    [Header("Attack Stats")]
    public Vector2 attackSize = new Vector2(0.5f, 5f);
    public int attackDamage = 40;
    public float meleeAttackRate = 2f;

    [Header("Melee Knockback Dealt")]
    public Vector2 knockbackVelocity;
    public float knockbackLength;

    [Header("Ground Slam Variables")]
    public Vector2 groundSlamArea;
    public int groundSlamDamage;
    public Vector2 groundSlamKnockbackVelocity;
    public float groundslamKnockbackLength;
    public float groundSlamAttackRate = 10f;
}
