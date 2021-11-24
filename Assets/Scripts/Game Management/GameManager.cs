using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector3 currentSpawnpoint;

    public Player player;

    public bool redScarf;

    public List<int> unlockedRooms = new List<int>();

    public bool hasBaseballBat;

    public Dictionary<int, bool> pickupCollectedDatabase = new Dictionary<int, bool>();

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        player = FindObjectOfType<Player>();
        if(player)
        {
            currentSpawnpoint = player.transform.position;
        }
        Time.timeScale = 1;
        if(player != null)
        {
            GameEvents.Instance.LoadGame();
        }
    }
    public void SetCurrentCheckpoint(Vector3 checkpointPos)
    {
        currentSpawnpoint = checkpointPos;
        GameEvents.Instance.SaveGame();
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