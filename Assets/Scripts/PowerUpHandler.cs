using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{
    Player player;
    RedScarfPlayer redScarf;

    private void Awake()
    {
        player = GetComponent<Player>();
        redScarf = GetComponent<RedScarfPlayer>();
    }

    public void GiveBaseballBat()
    {
        redScarf.GainBaseballBat();
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
}
