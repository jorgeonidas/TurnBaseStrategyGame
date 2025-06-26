using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static EventHandler OnAnyActionPointsChanged;
    public static EventHandler OnAnyUnitSpawned;
    public static EventHandler OnAnyUnitDead;
    private const int MAX_ACTION_POINTS = 2;
    [SerializeField] private bool _isEnemy;
    HealthSystem _healthSystem;
    GridPosition _gridPosition;
    private BaseAction[] _baseActionArray;
    private int _actionPoints = MAX_ACTION_POINTS;
    public bool IsEnemy => _isEnemy;
    void Awake()
    {
        _baseActionArray = GetComponents<BaseAction>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        Debug.Log($"Unit grid position: {_gridPosition.ToString()}");
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _healthSystem.OnDead += HealthSystem_OnDead;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        _healthSystem.OnDead -= HealthSystem_OnDead;
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != _gridPosition)
        {
            //this unit has changed position
            GridPosition oldGridPosition = _gridPosition;
            _gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    //Generic function to return a type of BaseAction
    public T GeatAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in _baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return _baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionsPointsCost());
            return true;
        }
        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return _actionPoints >= baseAction.GetActionsPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        _actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints() => _actionPoints;
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy && TurnSystem.Instance.IsPlayerTurn()))
        {
            _actionPoints = MAX_ACTION_POINTS;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Damage(int damageAmount)
    {
        _healthSystem.TakeDamage(damageAmount);
    }

    public float GetHealthNormalized()
    {
        return _healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}
