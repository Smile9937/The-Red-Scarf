using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Primary Menu")]
    [SerializeField] private Animator theButtonAnimator;
    [SerializeField] int maxMenuValue = 0;
    [SerializeField] int maxOptionsMenuValue = 5;
    [SerializeField] int maxKeyBindingValue = 11;

    int currentMenuValue = 0;
    int currentOptMenuValue = 0;

    [Header("Menu State")]
    [SerializeField] bool hasSelectedUI = false;
    [SerializeField] int currentMenu = 0;
    private MainMenuState menuState;
    [SerializeField]
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
            case MainMenuState.Keybindings:
                HandleKeybindingsMenuMovement();
                break;
            default:
                break;
        }
    }

    private void HandleMenuMovement()
    {
        if (theButtonAnimator.GetInteger("menuValue") <= maxMenuValue && InputManager.Instance.GetKeyDown(KeybindingActions.Down) && !hasSelectedUI)
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue + 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Up) && !hasSelectedUI)
        {
            currentMenuValue = Mathf.Clamp(currentMenuValue - 1, 0, maxMenuValue);
            SwitchWithinMenu(currentMenuValue);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
        {
            if (!hasSelectedUI)
            {
                theButtonAnimator.SetTrigger("pressButton");
            }
            else
            {
                ToggleSelectedUI();
            }
        }
    }
    private void HandleOptionsMenuMovement()
    {
        if (theButtonAnimator != null)
        {
            if (theButtonAnimator.GetInteger("menuValue") <= maxOptionsMenuValue && InputManager.Instance.GetKeyDown(KeybindingActions.Down) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue + 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Up) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue - 1, 0, maxOptionsMenuValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                if (!hasSelectedUI)
                {
                    theButtonAnimator.SetTrigger("pressButton");
                }
                else
                {
                    ToggleSelectedUI();
                }
            }
        }
    }
    private void HandleKeybindingsMenuMovement()
    {
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Left))
        {
            ReturnMovementBoolToNormal();
            theButtonAnimator.SetBool("moveLeft", true);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Right))
        {
            ReturnMovementBoolToNormal();
            theButtonAnimator.SetBool("moveRight", true);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Up))
        {
            ReturnMovementBoolToNormal();
            theButtonAnimator.SetBool("moveUp", true);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Down))
        {
            ReturnMovementBoolToNormal();
            theButtonAnimator.SetBool("moveDown", true);
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
        {
            if (!hasSelectedUI)
            {
                theButtonAnimator.SetTrigger("pressButton");
            }
            else
            {
                ToggleSelectedUI();
            }
        }
        /*
        if (theButtonAnimator != null)
        {
            if (theButtonAnimator.GetInteger("menuValue") <= maxKeyBindingValue && InputManager.Instance.GetKeyDown(KeybindingActions.Down) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue + 1, 0, maxKeyBindingValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Right) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue + 2, 0, maxKeyBindingValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Up) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue - 1, 0, maxKeyBindingValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (theButtonAnimator.GetInteger("menuValue") > 0 && InputManager.Instance.GetKeyDown(KeybindingActions.Left) && !hasSelectedUI)
            {
                currentOptMenuValue = Mathf.Clamp(currentOptMenuValue - 2, 0, maxKeyBindingValue);
                SwitchWithinMenu(currentOptMenuValue);
            }
            else if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack))
            {
                if (!hasSelectedUI)
                {
                    theButtonAnimator.SetTrigger("pressButton");
                }
                else
                {
                    ToggleSelectedUI();
                }
            }
        }
        */
    }

    private void ReturnMovementBoolToNormal()
    {
        if (IsInvoking("ReturnMovementBoolToNormal"))
            CancelInvoke("ReturnMovementBoolToNormal");

        theButtonAnimator.SetBool("moveUp", false);
        theButtonAnimator.SetBool("moveDown", false);
        theButtonAnimator.SetBool("moveLeft", false);
        theButtonAnimator.SetBool("moveRight", false);
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
                currentOptMenuValue = 0;
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
                currentOptMenuValue = 3;
                break;
            default:
                break;
        }
    }

    public void SwitchWithinMenu(int theState)
    {
        theButtonAnimator.SetInteger("menuValue", theState);
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
                break;
            case MainMenuState.Keybindings:
                ReturnBoolToNormal();
                theButtonAnimator.SetBool("isInMenu", false);
                menuState = MainMenuState.Keybindings;
                SwitchWithinMenu(currentMenuValue);
                break;
            default:
                break;
        }
    }

    private void ToggleSelectedUI()
    {
        hasSelectedUI = !hasSelectedUI;
    }


    private void SetIsInMenu()
    {
        theButtonAnimator.SetBool("isInMenu", true);
    }
}
