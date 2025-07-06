using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemHex<TGridObject>
{
    private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    private int _width;
    private int _height;
    private float _cellSize;
    private TGridObject[,] _gridObjectsArray;

    // Constructor para inicializar el grid
    // width: ancho del grid en celdas en el eje X 
    // height: alto del grid en celdas en el eje Z
    // cellSize: tamaño de cada celda en unidades del mundo
    // Constructor delegate for a TGridObject
    public GridSystemHex(int width, int height, float cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _gridObjectsArray = new TGridObject[width, height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _gridObjectsArray[x, z] = createGridObject(this, new GridPosition(x, z));
            }
        }
    }

    //coordenada en el mundo
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        //chequear si la fila es impar para aplicar offset en X (mitad del tamaño de la celda)
        Vector3 horizontalOffset = Vector3.zero;
        if (gridPosition.z % 2 != 0)
        {
            horizontalOffset = new Vector3(1, 0, 0) * _cellSize * 0.5f;
        }
        return new Vector3(gridPosition.x, 0, 0) * _cellSize 
        //Cada Z se posiciona al 75% del total del tamaño de la celda
                + new Vector3(0, 0, gridPosition.z) * _cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER + horizontalOffset;
    }

    //coordenada en el grid 
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        GridPosition roughXZ = new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.z / _cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER)
        );
        bool oddRow = roughXZ.z % 2 != 0;
        List<GridPosition> neighbourGridPositionList = new List<GridPosition>
        {
            //left and rigth
            roughXZ + new GridPosition(-1, 0),
            roughXZ + new GridPosition(+1, 0),
            //Up and down
            roughXZ + new GridPosition(0, +1),
            roughXZ + new GridPosition(0, -1),

            //diagonal, if odd we grab rigth neigborns else we grab left neighborns
            roughXZ + new GridPosition(oddRow ? +1 : -1, +1),
            roughXZ + new GridPosition(oddRow ? +1 : -1, -1),
        };

        //find which one is the closest
        GridPosition closestGridPosition = roughXZ;
        foreach (GridPosition neighbourGridPosition in neighbourGridPositionList)
        {
            if (Vector3.Distance(worldPosition, GetWorldPosition(neighbourGridPosition)) <
            Vector3.Distance(worldPosition, GetWorldPosition(closestGridPosition)))
            {
                closestGridPosition = neighbourGridPosition;
            }
        }

        return closestGridPosition;
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
    public TGridObject GetGridObject(GridPosition gridPosition)
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
    public int GetHeight() => _height;
}
