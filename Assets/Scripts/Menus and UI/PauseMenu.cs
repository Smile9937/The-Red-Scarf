using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private Animator backgroundDim;
    public bool gamePaused;

    private static PauseMenu instance;
    public static PauseMenu Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

    private void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;

        PlayBackgroundDim(true);
    }
    private void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;

        PlayBackgroundDim(false);
    }
    private void PlayBackgroundDim(bool menuOpen)
    {
        backgroundDim.SetBool("BackgroundDimOn", menuOpen);
        buttons.gameObject.SetActive(menuOpen);
    }

    public void ContinueGame()
    {
        gamePaused = false;
        ResumeGame();
    }
    public void OpenSettings()
    {
        Debug.Log("Open Settings");
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}