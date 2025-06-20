using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionPointsText;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Unit _unit;
    [SerializeField] private HealthSystem _healthSystem;
    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        _healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
        _healthSystem.OnDamaged -= HealthSystem_OnDamaged;
    }

    private void UpdateActionPointsText()
    {
        _actionPointsText.text = _unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        _healthBarImage.fillAmount = _healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
}
