using System;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, IInteractable
{
    private GridPosition _gridPosition;
    private Action _onInteractionCompleted;
    [SerializeField] private bool _isOpen;
    [SerializeField] private Transform _doorLeft;
    [SerializeField] private Transform _doorRigth;
    [SerializeField] private float _doorSequenceDuration = 1f;
    Sequence _doorSequence;
    float _openScale = 0.1f;
    float _closedScale = 1f;
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this);
        DoorSetup();
    }

    private void DoorSetup()
    {
        Vector3 doorLScale = _doorLeft.localScale;
        Vector3 doorRScale = _doorRigth.localScale;
        if (_isOpen)
        {
            doorLScale.x = _openScale;
            doorRScale.x = -_openScale;

        }
        else
        {
            doorLScale.x = _closedScale;
            doorRScale.x = -_closedScale;
        }
        _doorLeft.localScale = doorLScale;
        _doorRigth.localScale = doorRScale;
    }

    public void Intearact(Action onInteractionCompleted)
    {
        _onInteractionCompleted = onInteractionCompleted;
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
        _doorSequence.Append(_doorLeft.DOScaleX(_openScale, _doorSequenceDuration)).Join(_doorRigth.DOScaleX(-_openScale, _doorSequenceDuration)).OnComplete(() =>
        {
            SetDoorPositionWalkable(_isOpen);
            _onInteractionCompleted?.Invoke();
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
        _doorSequence.Append(_doorLeft.DOScaleX(_closedScale, _doorSequenceDuration)).Join(_doorRigth.DOScaleX(-_closedScale, _doorSequenceDuration)).OnComplete(() =>
        {
            SetDoorPositionWalkable(_isOpen);
            _onInteractionCompleted?.Invoke();
        });
    }

    private void SetDoorPositionWalkable(bool isOpen)
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, _isOpen);
    }
}
