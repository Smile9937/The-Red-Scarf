using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text composureText;
    [SerializeField] private Text rageText;

    private static PlayerUI instance;
    public static PlayerUI Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
        SetTextField();
    }

    private void SetTextField()
    {
        if (GameManager.Instance.redScarf)
        {
            rageText.gameObject.SetActive(true);
            composureText.gameObject.SetActive(false);
        }
        else
        {
            rageText.gameObject.SetActive(false);
            composureText.gameObject.SetActive(true);
        }
    }

    public void SetHealthText(int health)
    {
        healthText.text = "Health: " + health;
    }
    public void SetComposureText(int composure)
    {
        composureText.text = "Composure: " + composure;
    }
    public void SetRageText(int rage)
    {
        rageText.text = "Rage: " + rage;
    }

    private void Update()
    {
        SetTextField();
    }
}
