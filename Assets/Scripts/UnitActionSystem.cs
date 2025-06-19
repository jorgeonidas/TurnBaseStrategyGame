using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler OnActionStarted;
    public EventHandler<bool> OnBusyChanged;
    [SerializeField] LayerMask _unitLayerMask;
    [SerializeField] Unit _selecetedUnit;
    private BaseAction _selectedAction;
    private bool _isBusy;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one UnitActionSystem in the scene {transform} {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(_selecetedUnit);
    }

    private void Update()
    {
        if (_isBusy)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandlerUnitSelection())
        {
            return;
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (Input.GetMouseButton(0))
        {
            GridPosition mouseGridposition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (!_selectedAction.IsValidActionGridPosition(mouseGridposition))
            {
                return;
            }
            if (!_selecetedUnit.TrySpendActionPointsToTakeAction(_selectedAction))
            {
                return;
            }

            SetBusy();
            _selectedAction.TakeAction(mouseGridposition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        _isBusy = true;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    private void ClearBusy()
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    private bool TryHandlerUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _unitLayerMask))
            {
                if (hit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == _selecetedUnit)
                    {
                        //this unit is already selected
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        _selecetedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        _selectedAction = baseAction;
        Debug.Log($"Selected action: {_selectedAction.GetActionName()}");
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => _selecetedUnit;

    public BaseAction GetSelectedAction() => _selectedAction;
}
