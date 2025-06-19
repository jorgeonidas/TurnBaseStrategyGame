using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }
    [SerializeField] private Transform _gridSystemVisualSingle;
    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one GridSystemVisual in the scene {transform} {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        int gridW = LevelGrid.Instance.GetWidth();
        int gridH = LevelGrid.Instance.GetHeigth();
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[gridW, gridH];
        for (int x = 0; x < gridW; x++)
        {
            for (int z = 0; z < gridH; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualTransfor =
                Instantiate(_gridSystemVisualSingle, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                _gridSystemVisualSingleArray[x, z] = gridSystemVisualTransfor.GetComponent<GridSystemVisualSingle>();
            }
        }
        HideAllGridPositions();
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < _gridSystemVisualSingleArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridSystemVisualSingleArray.GetLength(1); z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions)
    {
        HideAllGridPositions();
        for (int i = 0; i < gridPositions.Count; i++)
        {
            _gridSystemVisualSingleArray[gridPositions[i].x, gridPositions[i].z].Show();
        }
    }

    public void UpdateGridVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedAction)
        {
            ShowGridPositionList(selectedAction.GetValidActionGridPositionList());
        }
    }
}
