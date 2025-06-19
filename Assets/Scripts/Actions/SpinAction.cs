using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float _totalSpinAmmount;
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        _totalSpinAmmount += spinAddAmmount;
        if (_totalSpinAmmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _totalSpinAmmount = 0;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        return new List<GridPosition>()
        {
            unitGridPosition
        };
    }

    public override int GetActionsPointsCost()
    {
        return 2;
    }
}
