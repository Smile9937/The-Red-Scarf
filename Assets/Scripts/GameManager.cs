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

    [Serializable]
    public class PlayerStats
    {
        public PlayerCharacterEnum playerCharacter;
        public float speed;
        public float jumpForce;
    }

    [HideInInspector] public  PlayerStats currentPlayer;

    public bool hasBaseballBat;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SavePlayerStats();
        }
        currentSpawnpoint = player.transform.position;
    }
    private void SavePlayerStats()
    {
        currentPlayer.playerCharacter = player.currentCharacter.playerCharacter;
        currentPlayer.speed = player.currentCharacter.speed;
        currentPlayer.jumpForce = player.currentCharacter.jumpForce;
        hasBaseballBat = player.hasBaseballBat;
    }

    public void LoadPlayerStats()
    {
        player.currentCharacter.playerCharacter = currentPlayer.playerCharacter;
        player.currentCharacter.speed = currentPlayer.speed;
        player.currentCharacter.jumpForce = currentPlayer.jumpForce;
        player.hasBaseballBat = hasBaseballBat;

        player.SetCurrentCharacter();
    }

    public void SetCurrentCheckpoint(Vector3 checkpointPos)
    {
        currentSpawnpoint = checkpointPos;
        SavePlayerStats();
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
