using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{

    private enum State
    {
        WaitForEnemyTurn,
        TakingTurn,
        Busy,
    }
    private State _state;
    private float _timer;

    private void Awake()
    {
        _state = State.WaitForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (_state)
        {

            case State.WaitForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    if (TryTakeEnemyIAAction(SetStateTakingTurn))
                    {
                        _state = State.Busy;
                    }
                    else
                    {
                        //no more enemyes have actions they can take
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            _state = State.TakingTurn;
            _timer = 2f;
        }

    }

    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _state = State.TakingTurn;
    }

    private bool TryTakeEnemyIAAction(Action onEnemyAIActionComplete)
    {
        Debug.Log($"Taking Enemy IA Action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitsList())
        {
            if (TryTakeEnemyIAAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyIAAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyIAAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                //enemy cannot afford this action
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyIAAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyIAAction testEnemyIAAction = baseAction.GetBestEnemyIAAction();
                if (testEnemyIAAction != null && testEnemyIAAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyIAAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
