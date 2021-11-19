using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private static GameEvents instance;
    public static GameEvents Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
    }

    public event Action<int> onActivate;
    public void Activate(int id)
    {
        onActivate?.Invoke(id);
    }

    public event Action<int> onDeActivate;
    public void DeActivate(int id)
    {
        onDeActivate?.Invoke(id);
    }

    public event Action onSaveGame;
    public void SaveGame()
    {
        onSaveGame?.Invoke();
    }

    public event Action onLoadGame;
    public void LoadGame()
    {
        onLoadGame?.Invoke();
    }

    public event Action onTripleBossMoveToStart;

    public void TripleBossMoveToStart()
    {
        onTripleBossMoveToStart?.Invoke();
    }

    public event Action onTripleBossPatternEnd;

    public void TripleBossPatternEnd()
    {
        onTripleBossPatternEnd?.Invoke();
    }
}