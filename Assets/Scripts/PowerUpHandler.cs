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
        player.currentHealth += healAmount;
    }
}
