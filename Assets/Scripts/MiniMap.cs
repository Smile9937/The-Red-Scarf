using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Image[] images;
    public List<int> unlockedRooms = new List<int>();

    private static MiniMap instance;
    public static MiniMap Instance { get { return instance; } }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            instance = this;
        }
    }
    void Start()
    {
        for(int i = 0; i < images.Length; i++)
        {
            if(unlockedRooms.Contains(i))
            {
                SetRoomActive(i);
            }
        }
    }

    public void UnlockRoom(int roomId)
    {
        unlockedRooms.Add(roomId);
        SetRoomActive(roomId);
    }

    private void SetRoomActive(int roomId)
    {
        images[roomId].gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        GameEvents.Instance.onSaveGame += SaveGame;
        GameEvents.Instance.onLoadGame += LoadGame;
    }
    private void OnDisable()
    {
        GameEvents.Instance.onSaveGame -= SaveGame;
        GameEvents.Instance.onLoadGame -= LoadGame;
    }
    public void SaveGame()
    {
        GameManager.Instance.unlockedRooms = new List<int>(unlockedRooms);
    }
    public void LoadGame()
    {
        unlockedRooms = new List<int>(GameManager.Instance.unlockedRooms);
    }
}
