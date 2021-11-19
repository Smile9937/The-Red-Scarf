using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Animator backgroundDim;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject soundSettings;
    [SerializeField] private GameObject keyBindingsSettings;
    [SerializeField] private GameObject miniMap;
    public bool gamePaused;

    private GameObject currentMenu;

    public Text[] keybindTexts;

    private MenuEnum menuEnum;

    private static PauseMenu instance;
    public static PauseMenu Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != this && instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (InputManager.Instance.waitingForInput)
            return;
        if (InputManager.Instance.GetKeyDown(KeybindingActions.OpenMenu))
        {
            gamePaused = !gamePaused;

            if(gamePaused)
            {
                PauseGame();
            } else
            {
                ResumeGame();
            }
        }
    }
    public void InvokeMenu(MenuInvoker currState)
    {
        if (InputManager.Instance.waitingForInput)
            return;
        menuEnum = currState.menuEnum;
        switch (menuEnum)
        {
            case MenuEnum.CloseMenu:
                ContinueGame();
                break;
            case MenuEnum.PauseMenu:
                currentMenu.SetActive(false);
                currentMenu = pauseMenu;
                currentMenu.SetActive(true);
                break;
            case MenuEnum.OptionsMenu:
                currentMenu.SetActive(false);
                currentMenu = settingsMenu;
                currentMenu.SetActive(true);
                break;
            case MenuEnum.SoundSettings:
                currentMenu.SetActive(false);
                currentMenu = soundSettings;
                currentMenu.SetActive(true);
                break;
            case MenuEnum.KeybindingsSettings:
                currentMenu.SetActive(false);
                currentMenu = keyBindingsSettings;
                currentMenu.SetActive(true);
                InputManager.Instance.SetText();
                break;
        }
    }

    private void PauseGame()
    {
        currentMenu = pauseMenu;
        Time.timeScale = 0;

        PlayBackgroundDim(true);
    }
    public void ContinueGame()
    {
        gamePaused = false;
        ResumeGame();
    }
    private void ResumeGame()
    {
        currentMenu.SetActive(false);
        Time.timeScale = 1;

        PlayBackgroundDim(false);
    }
    private void PlayBackgroundDim(bool menuOpen)
    {
        backgroundDim.SetBool("BackgroundDimOn", menuOpen);
        pauseMenu.SetActive(menuOpen);
        miniMap.SetActive(menuOpen);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeKeyBindings(KeybindInvoker currentKeybind)
    {
        InputManager.Instance.SetKeyBind(currentKeybind);
    }

    public void SetKeyBindingsText(KeyCode keyCode, int keybindId)
    {
        keybindTexts[keybindId].text = keyCode.ToString();
    }
}