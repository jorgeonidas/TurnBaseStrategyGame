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
    [SerializeField] private GameObject _enemyTurnVisualGameObject;

    private void Start()
    {
        UpdateTurnNumberText();
        _nextTurnButton.onClick.AddListener(NextTurn);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnNumberText()
    {
        _turnNumberText.text = $"Turn: {TurnSystem.Instance.GetTurnNumber()}";
    }

    private void UpdateEnemyTurnVisual()
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        _nextTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }

    public void NextTurn()
    {
        TurnSystem.Instance.NextTurn();
    }

}
