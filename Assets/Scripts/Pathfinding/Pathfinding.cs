using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    [SerializeField] Transform _gridDebugObjectPrefab;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _floorLayerMask;
    private int _width;
    private int _height;
    private float _cellSize;
    private int _floorAmount;
    List<GridSystem<PathNode>> _gridSystemList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one Pathfinding in the scene {transform} {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize, int floorAmount)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _floorAmount = floorAmount;
        _gridSystemList = new List<GridSystem<PathNode>>();
        for (int floor = 0; floor < _floorAmount; floor++)
        {
            GridSystem<PathNode> gridSystem =
            new GridSystem<PathNode>(_width, _height, _cellSize, floor, LevelGrid.FLOOR_HEIGTH,
                                    (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            _gridSystemList.Add(gridSystem);
        }

        //_gridSystem.CreateDebugObjects(_gridDebugObjectPrefab);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                for (int floor = 0; floor < _floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float rcOffsetDistance = 1;

                    //Unwalkable by default
                    GetNode(x, z, floor).SetIsWalkable(false);

                    //rc up to down checking floor
                    if (Physics.Raycast(worldPosition + Vector3.up * rcOffsetDistance,
                                Vector3.down,
                                rcOffsetDistance * 2, _floorLayerMask))
                    {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }

                    //rc down to up checking obstacles
                    if (Physics.Raycast(worldPosition + Vector3.down * rcOffsetDistance,
                                        Vector3.up,
                                        rcOffsetDistance * 2, _obstaclesLayerMask))
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                for (int floor = 0; floor < _floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                // Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //node already checked
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                //skip unwalkable nodess
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                //found a cheaper node add to openList
                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private GridSystem<PathNode> GetGridSystem(int floor)
    {
        return _gridSystemList[floor];
    }

    private PathNode GetNode(int x, int z, int floor)
    {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
            if (gridPosition.z - 1 >= 0)
            {
                // Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
            }

            if (gridPosition.z + 1 < _height)
            {
                // Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
            }
        }

        if (gridPosition.x + 1 < _width)
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
            if (gridPosition.z - 1 >= 0)
            {
                // Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
            }
            if (gridPosition.z + 1 < _height)
            {
                // Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor));
        }
        if (gridPosition.z + 1 < _height)
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor));
        }

        List<PathNode> totalNeighbourList = new List<PathNode>();
        totalNeighbourList.AddRange(neighbourList);

        foreach (PathNode pathNode in neighbourList)
        {
            GridPosition neighbourGridPosition = pathNode.GetGridPosition();
            if (neighbourGridPosition.floor - 1 >= 0)
            {
                totalNeighbourList.Add(GetNode(neighbourGridPosition.x, neighbourGridPosition.z, neighbourGridPosition.floor - 1));
            }
            if (neighbourGridPosition.floor + 1 < _floorAmount)
            {
                totalNeighbourList.Add(GetNode(neighbourGridPosition.x, neighbourGridPosition.z, neighbourGridPosition.floor + 1));
            }
        }

        return totalNeighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public void SetIsWalkableInsideColliderBounds(BoxCollider _boxCollider, bool isWalkable)
    {
        Vector3 max = _boxCollider.bounds.max;
        Vector3 min = _boxCollider.bounds.min;
        GridPosition maxGrid = LevelGrid.Instance.GetGridPosition(new Vector3(max.x, 0, max.z));
        GridPosition minGrid = LevelGrid.Instance.GetGridPosition(new Vector3(min.x, 0, min.z));
        for (int x = minGrid.x; x <= maxGrid.x; x++)
        {
            for (int z = minGrid.z; z <= maxGrid.z; z++)
            {
                //todo get the floor where the collider it is
                Pathfinding.Instance.SetIsWalkableGridPosition(new GridPosition(x, z, 0), isWalkable);
            }
        }
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
