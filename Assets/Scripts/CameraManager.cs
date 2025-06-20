using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionCameraGameobject;
    private void Start()
    {
        BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart;
        BaseAction.OnAnyActionEnds += BaseAction_OnAnyActionEnds;
        HideActionCamera();
    }
    private void OnDestroy()
    {
        BaseAction.OnAnyActionStart -= BaseAction_OnAnyActionStart;
        BaseAction.OnAnyActionEnds += BaseAction_OnAnyActionEnds;
    }

    private void BaseAction_OnAnyActionEnds(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStart(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();
                Vector3 cameraCharacterHeigth = Vector3.up * 1.7f;
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;
                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeigth + shoulderOffset + (shootDirection * -1f);
                _actionCameraGameobject.transform.position = actionCameraPosition;
                _actionCameraGameobject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeigth);
                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        _actionCameraGameobject.SetActive(true);
    }

    private void HideActionCamera()
    {
        _actionCameraGameobject.SetActive(false);
    }
}
