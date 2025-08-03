using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _shootPointTranform;
    //TODO: Should make a weapons manager or somekind of inventory
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private Transform _swordTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStoptMoving += MoveAction_OnStoptMoving;
            moveAction.OnChangedFloorStarted += MoveAction_OnChangedFloorStarted;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving -= MoveAction_OnStartMoving;
            moveAction.OnStoptMoving -= MoveAction_OnStoptMoving;
        }
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot -= ShootAction_OnShoot;
        }
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionCompleted -= SwordAction_OnSwordActionCompleted;
            swordAction.OnSwordActionStarted -= SwordAction_OnSwordActionStarted;
        }
    }
    private void Start()
    {
        EquipRifle();
    }
    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        _animator.SetTrigger("SwordSlash");
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStoptMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        _animator.SetTrigger("Shoot");
        Transform bulletProjectileTransfor = Instantiate(_bulletProjectilePrefab, _shootPointTranform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransfor.GetComponent<BulletProjectile>();
        Vector3 targetUnitShootAtPosotion = e.targetUnit.GetWorldPosition();
        float shoulderOffset = 1.7f;//TODO: somekind of shoot at position
        targetUnitShootAtPosotion.y += shoulderOffset;//quickfix to not shoot enemy units on their feets
        bulletProjectile.Setup(targetUnitShootAtPosotion);
    }

    
    private void MoveAction_OnChangedFloorStarted(object sender, MoveAction.OnChangeFloorStartedEventArgs e)
    {
        if (e.targetGridPosition.floor > e.unitGridPosition.floor)
        {
            //jump
            _animator.SetTrigger("JumpUp");
        }
        else
        {
            //Drop
            _animator.SetTrigger("JumpDown");   
        }
    }

    private void EquipSword()
    {
        _swordTransform.gameObject.SetActive(true);
        _weaponTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        _weaponTransform.gameObject.SetActive(true);
        _swordTransform.gameObject.SetActive(false);
    }
}
