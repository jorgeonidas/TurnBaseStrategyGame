using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] Button _button;
    [SerializeField] GameObject _selected;
    BaseAction _baseAction;
    public void SetBaseAction(BaseAction baseAction)
    {
        _baseAction = baseAction;
        _textMeshPro.text = baseAction.GetActionName().ToUpper();
        _button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVissual()
    {
       _selected.SetActive(UnitActionSystem.Instance.GetSelectedAction() == _baseAction);
    }
}
