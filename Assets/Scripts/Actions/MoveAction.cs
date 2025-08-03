using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public EventHandler OnStartMoving;
    public EventHandler OnStoptMoving;
    [SerializeField] private int _maxMoveDistance = 4;
    private List<Vector3> _positionList;
    private int _currentPositionIndex;
    private float _speed = 5f;
    private float _rotationSpeed = 10f;
    private float _reachDistance = 0.1f;

    void Update()
    {
        if (!_isActive)
        {
            return;
        }

        Vector3 targetPosition = _positionList[_currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * _rotationSpeed);
        if (Vector3.Distance(transform.position, targetPosition) > _reachDistance)
        {
            transform.position += moveDirection * _speed * Time.deltaTime;
        }
        else
        {
            _currentPositionIndex++;
            if (_currentPositionIndex >= _positionList.Count)
            {
                OnStoptMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(_unit.GetGridPosition(), targetGridPosition, out int pathLength);
        _currentPositionIndex = 0;
        _positionList = new List<Vector3>();
        foreach (GridPosition position in gridPositionList)
        {
            _positionList.Add(LevelGrid.Instance.GetWorldPosition(position));
        }
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    /// <summary>
    /// Obtienes las posiciones válidas en un rango máximo
    /// </summary>
    /// <returns></returns>
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -_maxMoveDistance; x <= _maxMoveDistance; x++)
        {
            for (int z = -_maxMoveDistance; z <= _maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);//Offset en el mismo piso
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //check if inside grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (testGridPosition == unitGridPosition)
                {
                    //same grid position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnThisGridPosition(testGridPosition))
                {
                    //Grid position already occupy by another unit
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                //too far than valid length, paht distance is too long
                int pathfindinfDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > _maxMoveDistance * pathfindinfDistanceMultiplier)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyIAAction GetEnemyIAAction(GridPosition gridPosition)
    {
        //get amount of valid target in range from gridPosition, so its the best I'll move to it
        ShootAction shootAction = _unit.GeatAction<ShootAction>();
        int targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);
        return new EnemyIAAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
