using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform _trailRendererTransform;
    [SerializeField] private Transform _bulletHitVfxPrefab;
    private float _bulletSpeed = 200f;
    private Vector3 _targetPosition;
    public void Setup(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDir = (_targetPosition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition);
        transform.position += moveDir * Time.deltaTime * _bulletSpeed;
        float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition);

        //check if overshoot the target
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = _targetPosition;//snapt to target position
            _trailRendererTransform.parent = null;
            Instantiate(_bulletHitVfxPrefab, _targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
