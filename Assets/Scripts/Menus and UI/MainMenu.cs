using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Primary Menu")]
    [SerializeField] private Animator theButtonAnimator;
    [SerializeField] int maxMenuValue = 0;
    [SerializeField] int maxOptionsMenuValue = 5;

    int currentMenuValue = 0;
    int currentOptMenuValue = 0;

    [Header("Menu State")]
    [SerializeField] int currentMenu = 0;
    private MainMenuState menuState;
    
    private enum MainMenuState
    {
        Main,
        Options,
        Keybindings,
    };


    private void Awake()
    {
        ActivateTheMenu(MainMenuState.Main);
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
        if (theButtonAnimator.GetInteger("menuValue") <= maxMenuValue && InputManager.Instance.GetKeyDown(KeybindingActions.Down))
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue + 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Up))
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue - 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
        {
            theButtonAnimator.SetTrigger("pressButton");
        }
    }
    private void HandleOptionsMenuMovement()
    {
        if (theButtonAnimator != null)
        {
            if (theButtonAnimator.GetInteger("menuValue") <= maxOptionsMenuValue && InputManager.Instance.GetKeyDown(KeybindingActions.Down))
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue + 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Up))
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue - 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                theButtonAnimator.SetTrigger("pressButton");
            }
        }
    }

    private void ReturnBoolToNormal()
    {
        theButtonAnimator.SetBool("isActive", false);
        theButtonAnimator.SetBool("isOptActive", false);
        theButtonAnimator.SetBool("isKeyActive", false);
        Invoke("FinalReturnToNormal", 0.01f);
    }

    private void FinalReturnToNormal()
    {
        switch (menuState)
        {
            case MainMenuState.Main:
                theButtonAnimator.SetBool("isActive", true);
                theButtonAnimator.SetBool("isOptActive", false);
                theButtonAnimator.SetBool("isKeyActive", false);
                break;
            case MainMenuState.Options:
                theButtonAnimator.SetBool("isActive", false);
                theButtonAnimator.SetBool("isOptActive", true);
                theButtonAnimator.SetBool("isKeyActive", false);
                break;
            case MainMenuState.Keybindings:
                theButtonAnimator.SetBool("isActive", false);
                theButtonAnimator.SetBool("isOptActive", false);
                theButtonAnimator.SetBool("isKeyActive", true);
                break;
            default:
                break;
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
                theButtonAnimator.SetInteger("menuValue", theState);
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
                ReturnBoolToNormal();
                theButtonAnimator.SetBool("isInMenu", false);
                menuState = MainMenuState.Main;
                SwitchWithinMenu(currentMenuValue);
                currentMenu = 0;
                break;
            case MainMenuState.Options:
                ReturnBoolToNormal();
                theButtonAnimator.SetBool("isInMenu", false);
                menuState = MainMenuState.Options;
                SwitchWithinMenu(currentOptMenuValue);
                currentMenu = 1;
                break;
            case MainMenuState.Keybindings:
                ReturnBoolToNormal();
                theButtonAnimator.SetBool("isInMenu", false);
                currentMenu = 2;
                break;
            default:
                break;
        }
    }


    private void SetIsInMenu()
    {
        theButtonAnimator.SetBool("isInMenu", true);
    }
}
