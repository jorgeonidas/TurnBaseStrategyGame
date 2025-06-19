using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnNumberText;
    [SerializeField] private Button _nextTurnButton;

    private void Start()
    {
        UpdateTurnNumberText();
        _nextTurnButton.onClick.AddListener(NextTurn);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
    }

    private void UpdateTurnNumberText()
    {
        _turnNumberText.text = $"Turn: {TurnSystem.Instance.GetTurnNumber()}";
    }

    public void NextTurn()
    {
        TurnSystem.Instance.NextTurn();
    }

}
