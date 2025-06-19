using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootAction : BaseAction
{
    private enum States
    {
        Aiming,
        Shooting,
        CoolOff,
    }

    private States _state;
    private int _maxShootDistance = 5;
    private float _stateTimer;
    private Unit _targetUnit;
    private bool _canShootBullet;
    private float _aimRotationSpeed = 10f;

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        _stateTimer -= Time.deltaTime;

        switch (_state)
        {
            case States.Aiming:
                Vector3 shootDirection = (_targetUnit.GetWorldPosition() - _unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, shootDirection, Time.deltaTime * _aimRotationSpeed);
                break;
            case States.Shooting:
                if (_canShootBullet)
                {
                    Shoot();
                    _canShootBullet = false;
                }
                break;
            case States.CoolOff:
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
            case States.Aiming:
                _state = States.Shooting;
                float shootStateTime = 0.1f;
                _stateTimer = shootStateTime;
                break;
            case States.Shooting:
                _state = States.CoolOff;
                float coolOffStateTime = 0.5f;
                _stateTimer = coolOffStateTime;
                break;
            case States.CoolOff:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    private void Shoot()
    {
        _targetUnit.Damage();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //check if inside grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                //manhattan distance check
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > _maxShootDistance)
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
        ActionStart(onActionComplete);

        _targetUnit = LevelGrid.Instance.GetUnitOnThisGridPosition(gridPosition);
        _state = States.Aiming;
        float aimingStateTime = 1f;
        _stateTimer = aimingStateTime;

        _canShootBullet = true;
    }
}
