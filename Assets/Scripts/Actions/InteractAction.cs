using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractDistance = 1;
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
    }
    public override string GetActionName()
    {
        return "Interact";
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
        for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++)
        {
            for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //check if inside grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null)
                {
                    //no door to interact with
                    continue;
                }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        Debug.Log($"Interact Action");
        ActionStart(onActionComplete);
        interactable.Intearact(ActionComplete);
    }
}
