using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro _gCostText;
    [SerializeField] private TextMeshPro _hCostText;
    [SerializeField] private TextMeshPro _fCostText;

    private PathNode _pathNode;
    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject;

    }

    protected override void Update()
    {
        base.Update();
        _gCostText.text = _pathNode.GetGCost().ToString();
        _hCostText.text = _pathNode.GetHCost().ToString();
        _fCostText.text = _pathNode.GetFCost().ToString();
    }
}
