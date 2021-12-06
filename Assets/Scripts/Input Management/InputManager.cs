using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    static readonly KeyCode[] _keyCodes =
    Enum.GetValues(typeof(KeyCode))
        .Cast<KeyCode>()
        .Where(k => k < KeyCode.Mouse0)
        .ToArray();

    [Serializable]
    public class DefaultKeybinds
    {
        public KeybindingActions keybindingAction;
        public KeyCode keyCode;
    }
    public DefaultKeybinds[] defaultKeybinds;
    [Serializable]
    public class CustomKeybinds
    {
        public KeybindingActions keybindingAction;
        public KeyCode keyCode;
    }

    public CustomKeybinds[] keybinds;

    [Serializable]
    public class KeyCodeImages
    {
        public Sprite sprite;
        public KeyCode keyCode;
    }
    public KeyCodeImages[] keyCodeImages;

    public bool waitingForInput;
    KeybindInvoker currentKeybind;

    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
    }

    private void Start()
    {
        keybinds = new CustomKeybinds[defaultKeybinds.Length];

        for(int i = 0; i < defaultKeybinds.Length; i++)
        {
            keybinds[i] = new CustomKeybinds();
            keybinds[i].keybindingAction = defaultKeybinds[i].keybindingAction;
            keybinds[i].keyCode = defaultKeybinds[i].keyCode;
        }
    }
    public KeyCode GetKeyForAction(KeybindingActions keybindingAction)
    {
        foreach (CustomKeybinds keybindingCheck in keybinds)
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
        foreach (CustomKeybinds keybindingCheck in keybinds)
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
        foreach (CustomKeybinds keybindingCheck in keybinds)
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
        foreach (CustomKeybinds keybindingCheck in keybinds)
        {
            if (keybindingCheck.keybindingAction == key)
            {
                return Input.GetKeyUp(keybindingCheck.keyCode);
            }
        }
        return false;
    }
    public void ResetToDefault()
    {
        foreach(CustomKeybinds customKeybind in keybinds)
        {
            foreach(DefaultKeybinds defaultKeybind in defaultKeybinds)
            {
                if(customKeybind.keybindingAction == defaultKeybind.keybindingAction)
                {
                    if (SceneManager.GetActiveScene().buildIndex != 0)
                        if (Array.IndexOf(keybinds, customKeybind) >= PauseMenu.Instance.keyTexts.Length)
                            return;

                    customKeybind.keyCode = defaultKeybind.keyCode;
                    int keybindingId = Array.IndexOf(keybinds, customKeybind);
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {

                    } else
                    {
                        PauseMenu.Instance.SetKeyBindingsText(customKeybind.keybindingAction, customKeybind.keyCode);
                    }
                }
            }
        }
    }
    public void SetText()
    {
        foreach (CustomKeybinds customKeybind in keybinds)
        {
            if (Array.IndexOf(keybinds, customKeybind) >= PauseMenu.Instance.keyTexts.Length)
                return;
            PauseMenu.Instance.SetKeyBindingsText(customKeybind.keybindingAction, customKeybind.keyCode);
        }
    }
    public void SetKeyBind(KeybindInvoker keybind)
    {
        waitingForInput = true;
        currentKeybind = keybind;
    }
    private static IEnumerable<KeyCode> GetPressedKey()
    {
        if (Input.anyKeyDown)
        {
            for (int i = 0; i < _keyCodes.Length; i++)
                if (Input.GetKey(_keyCodes[i]))
                    yield return _keyCodes[i];
        }
    }
    private void Update()
    {
        if (!waitingForInput)
            return;

        if(Input.anyKeyDown)
        {
            KeyCode keyCode = GetPressedKey().FirstOrDefault();
            if (keyCode == KeyCode.None)
                return;

            foreach (CustomKeybinds customKeybind in keybinds)
            {
                if(customKeybind.keyCode == keyCode)
                {
                    if (SceneManager.GetActiveScene().buildIndex != 0)
                        if (Array.IndexOf(keybinds, customKeybind) >= PauseMenu.Instance.keyTexts.Length)
                            return;

                    foreach(CustomKeybinds checkDuplicate in keybinds)
                    {
                        if(currentKeybind.keyBindingsAction == checkDuplicate.keybindingAction)
                        {
                            customKeybind.keyCode = checkDuplicate.keyCode;
                            if (SceneManager.GetActiveScene().buildIndex == 0)
                            {

                            } else
                            {
                                PauseMenu.Instance.SetKeyBindingsText(customKeybind.keybindingAction, customKeybind.keyCode);
                            }
                            
                            checkDuplicate.keyCode = keyCode;
                            if (SceneManager.GetActiveScene().buildIndex == 0)
                            {

                            } else
                            {
                                PauseMenu.Instance.SetKeyBindingsText(checkDuplicate.keybindingAction, checkDuplicate.keyCode);
                            }
                        }
                    }
                }
            }
            foreach(CustomKeybinds customKeybind in keybinds)
            {
                if (customKeybind.keybindingAction == currentKeybind.keyBindingsAction)
                {
                    customKeybind.keyCode = keyCode;
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {

                    } else
                    {
                        PauseMenu.Instance.SetKeyBindingsText(customKeybind.keybindingAction, customKeybind.keyCode);
                    }
                    break;
                }
            }
            waitingForInput = false;
        }
    }
}
