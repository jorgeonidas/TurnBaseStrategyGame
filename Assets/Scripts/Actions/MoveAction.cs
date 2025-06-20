using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public EventHandler OnStartMoving;
    public EventHandler OnStoptMoving;
    [SerializeField] private int _maxMoveDistance = 4;
    private Vector3 _targetPosition;
    private float _speed = 5f;
    private float _rotationSpeed = 10f;
    private float _reachDistance = 0.1f;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive)
        {
            return;
        }
        Vector3 moveDirection = (_targetPosition - transform.position).normalized;
        if (Vector3.Distance(transform.position, _targetPosition) > _reachDistance)
        {
            transform.position += moveDirection * _speed * Time.deltaTime;
        }
        else
        {
            OnStoptMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * _rotationSpeed);
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        this._targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
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
                GridPosition offsetGridPosition = new GridPosition(x, z);
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
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
