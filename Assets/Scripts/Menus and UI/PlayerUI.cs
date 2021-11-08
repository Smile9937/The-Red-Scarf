using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text composureText;

    private static PlayerUI instance;
    public static PlayerUI Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }
    public void SetHealthText(int health)
    {
        healthText.text = "Health: " + health;
    }
    public void SetComposureText(int composure)
    {
        composureText.text = "Composure " + composure;
    }
}
