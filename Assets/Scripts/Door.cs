using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Door : MonoBehaviour, IInteractable
{
    private Action _onInteractionCompleted;
    [SerializeField] private bool _isOpen;
    [SerializeField] private Transform _doorLeft;
    [SerializeField] private Transform _doorRigth;
    [SerializeField] private float _doorSequenceDuration = 1f;
    private List<GridPosition> _doorGridPositions;
    private BoxCollider _doorColider;
    Sequence _doorSequence;
    float _openScale = 0.1f;
    float _closedScale = 1f;
    private void Awake()
    {
        _doorColider = GetComponent<BoxCollider>();//disble this collider when door is open
    }
    private void Start()
    {
        SetGridPositions();
        SetInteractablePositions();
        DoorSetup();
    }

    private void SetInteractablePositions()
    {
        for (int i = 0; i < _doorGridPositions.Count; i++)
        {
            LevelGrid.Instance.SetInteractableAtGridPosition(_doorGridPositions[i], this);
        }
    }

    private void SetGridPositions()
    {
        _doorGridPositions = new List<GridPosition>();
        //setup grid positions, these grid positions will be use for set interactivity and walkability
        Vector3 max = _doorColider.bounds.max;
        Vector3 min = _doorColider.bounds.min;
        GridPosition maxGrid = LevelGrid.Instance.GetGridPosition(new Vector3(max.x, 0, max.z));
        GridPosition minGrid = LevelGrid.Instance.GetGridPosition(new Vector3(min.x, 0, min.z));
        for (int x = minGrid.x; x <= maxGrid.x; x++)
        {
            for (int z = minGrid.z; z <= maxGrid.z; z++)
            {
                _doorGridPositions.Add(new GridPosition(x, z, 0));//TODO obener el piso donde esta ubicada la puerta
            }
        }
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
        SetDoorAreaWalkable(_isOpen);
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
            SetDoorAreaWalkable(_isOpen);
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
            SetDoorAreaWalkable(_isOpen);
            _onInteractionCompleted?.Invoke();
        });
    }

    private void SetDoorAreaWalkable(bool isOpen)
    {
        _doorColider.enabled = !isOpen;
        for (int i = 0; i < _doorGridPositions.Count; i++)
        {
            Pathfinding.Instance.SetIsWalkableGridPosition(_doorGridPositions[i], isOpen);
        }
    }
}
