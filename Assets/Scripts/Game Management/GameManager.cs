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

    private List<int> activeItems = new List<int>();

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
            //SavePlayerStats();
        }
        currentSpawnpoint = player.transform.position;
        GameEvents.instance.onActivate += Activate;
        GameEvents.instance.onDeActivate += DeActivate;
    }
    /*private void SavePlayerStats()
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
    }*/

    public void SetCurrentCheckpoint(Vector3 checkpointPos)
    {
        currentSpawnpoint = checkpointPos;
        //SavePlayerStats();
    }
    public void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        foreach(int active in activeItems)
        {
            GameEvents.instance.Activate(active);
        }
    }
    public void SwapCharacter()
    {
        redScarf = !redScarf;
    }

    private void Activate(int id)
    {
        activeItems.Add(id);
    }
    private void DeActivate(int id)
    {
        activeItems.Remove(id);
    }
}
