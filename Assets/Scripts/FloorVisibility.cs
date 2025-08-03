using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] bool _dynamicFloorPosition = false;
    [SerializeField] private Renderer[] _ignoreRendererList;
    private Renderer[] _rendererArray;
    private int _floor;
    private void Awake()
    {
        _rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        _floor = LevelGrid.Instance.GetFloor(transform.position);
        //if its grounded destroy this scritp
        if (_floor == 0 && !_dynamicFloorPosition)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        //change floor with a callback instead
        if (_dynamicFloorPosition)
        {
            _floor = LevelGrid.Instance.GetFloor(transform.position);
        }

        float cameraHeigth = CameraController.Instance.GetCameraHeigth();
        float floorHeigthOffset = 2f;
        bool showVisibleObjects = cameraHeigth > LevelGrid.FLOOR_HEIGTH * _floor + floorHeigthOffset;
        if (showVisibleObjects || _floor == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (Renderer renderer in _rendererArray)
        {
            if (_ignoreRendererList.Contains(renderer))
            {
                continue;
            }
            renderer.enabled = true;
        }
    }

    private void Hide()
    {
        foreach (Renderer renderer in _rendererArray)
        {
            if (_ignoreRendererList.Contains(renderer))
            {
                continue;
            }
            renderer.enabled = false;
        }
    }

    public void SetFloor(int floor)
    {
        _floor = floor;
    }
}
