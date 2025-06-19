using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem
{
    private int _width;
    private int _height;
    private float _cellSize;
    private GridObject[,] _gridObjectsArray;

    // Constructor para inicializar el grid
    // width: ancho del grid en celdas en el eje X 
    // height: alto del grid en celdas en el eje Z
    // cellSize: tamaño de cada celda en unidades del mundo
    public GridSystem(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _gridObjectsArray = new GridObject[width, height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _gridObjectsArray[x, z] = new GridObject(this, new GridPosition(x, z));
            }
        }
    }

    //coordenada en el mundo
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        //multiplicado por la dimension de la celda
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }

    //coordenada en el grid 
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / _cellSize), Mathf.RoundToInt(worldPosition.z / _cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                debugTransform.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    // Devuelve el objeto de la celda en la posición gridPosition
    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    //Si una posicion esta dentro de los límites del grid
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
                gridPosition.z >= 0 &&
                gridPosition.x < _width &&
                gridPosition.z < _height;
    }

    public int GetWidth() => _width;
    public int GetHeigth() => _height;
}
