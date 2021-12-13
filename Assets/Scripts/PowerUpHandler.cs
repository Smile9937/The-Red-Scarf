using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void GiveBaseballBat()
    {
        player.GainBaseballBat();
    }

    public void Heal(int healAmount)
    {
        if(healAmount + player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        } else
        {
            player.currentHealth += healAmount;
        }
    }

    public void IncreaseAttackBonus(int attackAmount)
    {
        player.attackBonus += attackAmount;
    }
    public void IncreaseSpeedBonus(float speedAmount)
    {
        player.speedBonus += speedAmount;
    }
}
