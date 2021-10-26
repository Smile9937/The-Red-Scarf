using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    float untilNextUpdate = 0;
    
    public void UpdateCharacterHP(int updatedHealth)
    {
        slider.value += updatedHealth;
    }
}
