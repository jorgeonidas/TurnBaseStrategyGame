using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro _debutText;
    private GridObject _gridObject;
    public void SetGridObject(GridObject gridObject)
    {
        _gridObject = gridObject;
        //Debug.Log($"GridDebugObject: {gridObject.ToString()}");
        _debutText.text = _gridObject.ToString();
    }

    void Update()
    {
        _debutText.text = _gridObject.ToString();
    }
}
