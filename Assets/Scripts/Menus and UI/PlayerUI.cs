using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthBar;
    [SerializeField] private Color theHealthColor;
    [SerializeField] private Color theHealthColorLow;
    [Header("Special Bar")]
    [SerializeField] private Slider rageSlider;

    private static PlayerUI instance;
    public static PlayerUI Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
    }

    public void SetHealthUI(int theHealth, int maxHealth)
    {
        if (theHealth != 0 || maxHealth != 0)
        {
            float theAmount = (float)theHealth / (float)maxHealth;
            healthSlider.value = theAmount;
            healthBar.color = Color.Lerp(theHealthColorLow, theHealthColor, theAmount);
        }
        else if (theHealth == 0)
        {
            healthSlider.value = 0;
            healthBar.color = theHealthColorLow;
        }
    }
    public void SetSpecialUI(int theRage, int maxRage)
    {
        if (theRage == 0 || maxRage == 0)
        {
            rageSlider.value = 0;
        }
        else
        {
            float theAmount = (float)theRage / (float)maxRage;
            rageSlider.value = theAmount;
        }
    }
}