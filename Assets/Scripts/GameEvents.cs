using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
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
}
