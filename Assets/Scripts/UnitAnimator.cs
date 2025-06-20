using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _shootPointTranform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStoptMoving += MoveAction_OnStoptMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
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
        targetUnitShootAtPosotion.y = _shootPointTranform.position.y;//quickfix to not shoot enemy units on their feets
        bulletProjectile.Setup(targetUnitShootAtPosotion);
    }
}
