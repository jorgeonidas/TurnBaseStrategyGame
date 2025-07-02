using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    private GridPosition _gridPosition;
    private Action _onInteractCompleted;
    [SerializeField] private bool _isOpen;
    [SerializeField] private Transform _doorLeft;
    [SerializeField] private Transform _doorRigth;

    Sequence _doorSequence;

    private void Awake()
    {
    }
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetDoorAtGridPosition(_gridPosition, this);
        DoorSetup();
    }

    private void DoorSetup()
    {
        Vector3 doorLScale = _doorLeft.localScale;
        Vector3 doorRScale = _doorRigth.localScale;
        if (_isOpen)
        {
            doorLScale.x = 0.1f;
            doorRScale.x = -0.1f;

        }
        else
        {
            doorLScale.x = 1f;
            doorRScale.x = -1f;
        }
        _doorLeft.localScale = doorLScale;
        _doorRigth.localScale = doorRScale;
    }

    public void Intearact(Action onInteractCompleted)
    {
        _onInteractCompleted = onInteractCompleted;
        if (!_isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        _isOpen = true;
        if (_doorSequence != null && _doorSequence.IsActive())
        {
            _doorSequence.Kill();
        }

        _doorSequence = DOTween.Sequence();
        _doorSequence.Append(_doorLeft.DOScaleX(0.1f, 1f)).Join(_doorRigth.DOScaleX(-0.1f, 1f)).OnComplete(() =>
        {
            SetDoorPositionWalkable(_isOpen);
            _onInteractCompleted?.Invoke();
        });
    }

    private void CloseDoor()
    {
        _isOpen = false;
        if (_doorSequence != null && _doorSequence.IsActive())
        {
            _doorSequence.Kill();
        }

        _doorSequence = DOTween.Sequence();
        _doorSequence.Append(_doorLeft.DOScaleX(1f, 1f)).Join(_doorRigth.DOScaleX(-1f, 1f)).OnComplete(() =>
        {
            SetDoorPositionWalkable(_isOpen);
            _onInteractCompleted?.Invoke();
        });
    }

    private void SetDoorPositionWalkable(bool isOpen)
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, _isOpen);
    }
}
