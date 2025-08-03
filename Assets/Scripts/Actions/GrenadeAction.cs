using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform _grenadeProjectilePrefab;
    private int _maxThrowDistance = 7;
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
    }
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyIAAction GetEnemyIAAction(GridPosition gridPosition)
    {
        return new EnemyIAAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++)
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //check if inside grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                //manhattan distance check
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > _maxThrowDistance)
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
        Debug.Log($"Grenade Action");
        Transform grenadeTransform = Instantiate(_grenadeProjectilePrefab, _unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourCompleted);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourCompleted()
    {
        ActionComplete();
    }
}
