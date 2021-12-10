using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class InputImages : MonoBehaviour
{
    [Serializable]
    public class InputSprite
    {
        public SpriteRenderer spriteRenderer;
        public KeybindingActions keybindingAction;
    }
    public InputSprite[] inputSprites;

    [Serializable]
    public class KeyCodeSprite
    {
        public Sprite activeSprite;
        public Sprite sprite;
        public KeyCode keyCode;
    }

    public KeyCodeSprite[] keyCodeSprites;

    private static InputImages instance;
    public static InputImages Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetKeybindImages()
    {
        InputManager.Instance.SetText();
    }

    public void ChangeActive(KeybindingActions keybindingAction, bool active)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            return;

        foreach(InputSprite sprite in inputSprites)
        {
            if(sprite.keybindingAction == keybindingAction)
            {
                foreach(KeyCodeSprite keyCodeSprite in keyCodeSprites)
                {
                    if(keyCodeSprite.sprite == sprite.spriteRenderer.sprite || keyCodeSprite.activeSprite == sprite.spriteRenderer.sprite)
                    {
                        if(active)
                        {
                            sprite.spriteRenderer.sprite = keyCodeSprite.activeSprite;
                        }
                        else
                        {
                            sprite.spriteRenderer.sprite = keyCodeSprite.sprite;
                        }
                        return;
                    }
                }
            }
        }
    }

    public void SetKeyBindingsImage(KeybindingActions keybindingAction, KeyCode keyCode)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            return;

        foreach(InputSprite sprite in inputSprites)
        {
            if(sprite.keybindingAction == keybindingAction)
            {
                foreach(KeyCodeSprite keyCodeSprite in keyCodeSprites)
                {
                    if (keyCodeSprite.keyCode == keyCode)
                    {
                        sprite.spriteRenderer.sprite = keyCodeSprite.sprite;
                        return;
                    }
                }
            }
        }
    }
}
