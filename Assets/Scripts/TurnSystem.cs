using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }
    public EventHandler OnTurnChanged;
    private int _turnNumber = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one TurnSystem in the scene {transform} {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NextTurn()
    {
        _turnNumber++;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber() => _turnNumber;
}
