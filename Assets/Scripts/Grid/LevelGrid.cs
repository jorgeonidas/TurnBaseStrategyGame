using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }
    public static readonly float FLOOR_HEIGTH = 3f;
    public event EventHandler OnAnyUnitMovedGridPosition;
    [SerializeField] private Transform _debugPrefab;

    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private int _floorAmount = 1;
    [SerializeField] private float _cellSize = 2;
    private List<GridSystem<GridObject>> _gridSystemList;
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one LevelGrid in the scene {transform} {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _gridSystemList = new List<GridSystem<GridObject>>();
        for (int floor = 0; floor < _floorAmount; floor++)
        {
            GridSystem<GridObject> gridSystem =
            new GridSystem<GridObject>(_width,
                                        _height,
                                        _cellSize,
                                        floor,
                                        FLOOR_HEIGTH,
                                        (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            _gridSystemList.Add(gridSystem);
        }
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(_width, _height, _cellSize);
    }

    private GridSystem<GridObject> GetGridSystem(int floor)
    {
        return _gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    //all floors will have the same size
    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeigth() => GetGridSystem(0).GetHeight();
    public int GetFloor(Vector3 worldPosition)
    {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGTH);
    }
    public int GetFloorAmount() => _floorAmount;
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(floor).GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);

    public bool HasAnyUnitOnThisGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitOnThisGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

    // public void SetInteractableInsideColliderBounds(BoxCollider boxCollider, IInteractable interactable)
    // {
    //     Vector3 max = boxCollider.bounds.max;
    //     Vector3 min = boxCollider.bounds.min;
    //     GridPosition maxGrid = LevelGrid.Instance.GetGridPosition(new Vector3(max.x, 0, max.z));
    //     GridPosition minGrid = LevelGrid.Instance.GetGridPosition(new Vector3(min.x, 0, min.z));
    //     for (int x = minGrid.x; x <= maxGrid.x; x++)
    //     {
    //         for (int z = minGrid.z; z <= maxGrid.z; z++)
    //         {
    //             SetInteractableAtGridPosition(new GridPosition(x, z), interactable);
    //         }
    //     }
    // }
}
