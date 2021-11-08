using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Keybindings keybindings;

    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }
    public KeyCode GetKeyForAction(KeybindingActions keybindingAction)
    {
        foreach (Keybindings.KeybindingCheck keybindingCheck in keybindings.keybindingChecks)
        {
            if (keybindingCheck.keybindingAction == keybindingAction)
            {
                return keybindingCheck.keyCode;
            }
        }
        return KeyCode.None;
    }

    public bool GetKeyDown(KeybindingActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindingCheck in keybindings.keybindingChecks)
        {
            if (keybindingCheck.keybindingAction == key)
            {
                return Input.GetKeyDown(keybindingCheck.keyCode);
            }
        }
        return false;
    }

    public bool GetKey(KeybindingActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindingCheck in keybindings.keybindingChecks)
        {
            if (keybindingCheck.keybindingAction == key)
            {
                return Input.GetKey(keybindingCheck.keyCode);
            }
        }
        return false;
    }
    public bool GetKeyUp(KeybindingActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindingCheck in keybindings.keybindingChecks)
        {
            if (keybindingCheck.keybindingAction == key)
            {
                return Input.GetKeyUp(keybindingCheck.keyCode);
            }
        }
        return false;
    }
}
