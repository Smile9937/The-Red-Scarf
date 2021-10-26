using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector3 currentSpawnpoint;

    private Player player;

    public bool redScarf;

    private static GameManager instance;
    public static GameManager Instance { get { return instance;} }
    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        player = FindObjectOfType<Player>();
        currentSpawnpoint = player.transform.position;
    }
    public void SetCurrentCheckpoint(Vector3 checkpointPos)
    {
        currentSpawnpoint = checkpointPos;
    }
    public void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SwapCharacter()
    {
        redScarf = !redScarf;

    }
}
