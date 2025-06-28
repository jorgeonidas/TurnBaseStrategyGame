using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition _gridPosition;
    private int _gCost;
    private int _hCost;
    private int _fCost;
    private PathNode _cameFromPathNode;
    private bool _isWalkable = true;
    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }

    public int GetGCost() => _gCost;
    public int GetHCost() => _hCost;
    public int GetFCost() => _fCost;
    public GridPosition GetGridPosition() => _gridPosition;
    public PathNode GetCameFromPathNode() => _cameFromPathNode;
    public bool IsWalkable() => _isWalkable;

    public void SetGCost(int cost)
    {
        _gCost = cost;
    }

    public void SetHCost(int cost)
    {
        _hCost = cost;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    public void ResetCameFromPathNode()
    {
        _cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        _cameFromPathNode = pathNode;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        _isWalkable = isWalkable;
    }
}
