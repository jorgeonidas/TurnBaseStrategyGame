using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    public enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }
    private int _maxSwordDistance = 1; //in grid cells
    private State _state;
    private float _stateTimer;
    private Unit _targetUnit;
    private float _rotateSpeed = 10f;

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        _stateTimer -= Time.deltaTime;

        switch (_state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 shootDirection = (_targetUnit.GetWorldPosition() - _unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, shootDirection, Time.deltaTime * _rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;
        }

        if (_stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.SwingingSwordBeforeHit:
                _state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                _stateTimer = afterHitStateTime;
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                _targetUnit.Damage(100);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyIAAction GetEnemyIAAction(GridPosition gridPosition)
    {
        return new EnemyIAAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -_maxSwordDistance; x <= _maxSwordDistance; x++)
        {
            for (int z = -_maxSwordDistance; z <= _maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //check if inside grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnThisGridPosition(testGridPosition))
                {
                    //Grid position is empty, no unit to shoot
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitOnThisGridPosition(testGridPosition);

                //If both are in same team
                if (targetUnit.IsEnemy == _unit.IsEnemy)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitOnThisGridPosition(gridPosition);
        _state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        _stateTimer = beforeHitStateTime;
        Debug.Log($"Sword Action");
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance() => _maxSwordDistance;
}
