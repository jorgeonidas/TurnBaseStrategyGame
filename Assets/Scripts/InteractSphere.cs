using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    private Action _onInteractionCompleted;
    [SerializeField] private Material _greenMat;
    [SerializeField] private Material _redMat;
    [SerializeField] private MeshRenderer _meshRenderer;
    GridPosition _gridPosition;
    private bool _isGreen;
    private float _timer;
    private bool _isActive;
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this);
        SetColorGreen();
    }
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _isActive = false;
            _onInteractionCompleted?.Invoke();
        }
    }
    private void SetColorGreen()
    {
        _isGreen = true;
        _meshRenderer.material = _greenMat;
    }

    private void SetColorRed()
    {
        _isGreen = false;
        _meshRenderer.material = _redMat;
    }

    public void Intearact(Action onInteractionCompleted)
    {
        _timer = 0.5f;
        _isActive = true;
        _onInteractionCompleted = onInteractionCompleted;
        if (_isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }

    public GridPosition GetGridPosition() => _gridPosition;
}
