using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem _gridSystem;
    private GridPosition _gridPosition;
    private List<Unit> _unitList;
    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        _unitList.Add(unit);
    }

    public List<Unit> GetUnitList()
    {
        return _unitList;
    }

    public void RemoveUnit(Unit unit)
    {
        _unitList.Remove(unit);
    }

    public bool HasAnyUnit()
    {
        return _unitList.Count > 0;
    }

    public override string ToString()
    {
        string units = "";
        foreach (Unit unit in _unitList)
        {
            units += unit.name + "\n";
        }
        return _gridPosition.ToString() + "\n" + units;
    }
}
