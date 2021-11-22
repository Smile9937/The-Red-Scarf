using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Primary Menu")]
    [SerializeField] private Animator theButtonAnimator;
    [SerializeField] int maxMenuValue = 0;
    [Header("Options Menu")]
    [SerializeField] private Animator theOptionsAnimator;
    [SerializeField] int maxOptionsMenuValue = 5;

    int currentMenuValue = 0;
    int currentOptMenuValue = 0;

    [Header("Menu State")]
    private MainMenuState menuState;
    
    private enum MainMenuState
    {
        Main,
        Options,
        Keybindings,
    };


    private void Awake()
    {
        SwitchWithinMenu(0);
    }

    private void Update()
    {
        switch (menuState)
        {
            case MainMenuState.Main:
                HandleMenuMovement();
                break;
            case MainMenuState.Options:
                HandleOptionsMenuMovement();
                break;
            default:
                break;
        }
    }

    private void HandleMenuMovement()
    {
        if (theButtonAnimator.GetInteger("menuValue") <= maxMenuValue && Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue + 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (theButtonAnimator.GetInteger("menuValue") > 0 && Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue - 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            theButtonAnimator.SetTrigger("pressButton");
            if (currentMenuValue == 1)
                ActivateTheMenu(MainMenuState.Options);
        }
    }
    private void HandleOptionsMenuMovement()
    {
        if (theOptionsAnimator != null)
        {
            if (theOptionsAnimator.GetInteger("menuValue") <= maxOptionsMenuValue && Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue + 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theOptionsAnimator.GetInteger("menuValue") > 0 && Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue - 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentOptMenuValue <= 0)
                    ActivateTheMenu(MainMenuState.Main);
            }
        }
    }

    public void SwitchWithinMenu(int theState)
    {
        switch (menuState)
        {
            case MainMenuState.Main:
                theButtonAnimator.SetInteger("menuValue", theState);
                break;
            case MainMenuState.Options:
                theOptionsAnimator.SetInteger("menuValue", theState);
                break;
            default:
                break;
        }
    }

    private void ActivateTheMenu(MainMenuState theState)
    {
        switch (theState)
        {
            case MainMenuState.Main:
                theButtonAnimator.SetBool("isActive", true);
                theOptionsAnimator.SetBool("isActive", false);
                menuState = MainMenuState.Main;
                SwitchWithinMenu(currentMenuValue);
                break;
            case MainMenuState.Options:
                theButtonAnimator.SetBool("isActive", false);
                theOptionsAnimator.SetBool("isActive", true);
                menuState = MainMenuState.Options;
                SwitchWithinMenu(currentOptMenuValue);
                break;
            case MainMenuState.Keybindings:
                break;
            default:
                break;
        }
    }
}
