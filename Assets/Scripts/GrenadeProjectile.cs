using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;
    private Action _onGrenadeBehaviourCompled;
    [SerializeField] private Transform _grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private AnimationCurve _arcYAnimationCurve;
    float _moveSpeed = 15f;
    float _damageRadius = 4f; //2 grid positions
    int _damage = 30;
    Vector3 _targetPosition;
    private float _totalDistance;
    private Vector3 _positionXZ;
    private float _reachTargetDistance = 0.2f;

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - _positionXZ).normalized;
        _positionXZ += moveDirection * Time.deltaTime * _moveSpeed;
        float distance = Vector3.Distance(_positionXZ, _targetPosition);
        float distanceNormalized = 1 - (distance / _totalDistance); //closer to target so the  close to 1
        float maxHeigth = _totalDistance / 4f;
        //get y position evaluating animation curve
        float positionY = _arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeigth;
        transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z);
        if (Vector3.Distance(transform.position, _targetPosition) < _reachTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, _damageRadius);
            foreach (Collider col in colliderArray)
            {
                if (col.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(_damage);
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            Instantiate(_grenadeExplodeVfxPrefab, _targetPosition + Vector3.up * 1f, Quaternion.identity);
            _trailRenderer.transform.parent = null;
            Destroy(gameObject);
            _onGrenadeBehaviourCompled?.Invoke();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourCompled)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        _onGrenadeBehaviourCompled = onGrenadeBehaviourCompled;
        _positionXZ = transform.position;
        _positionXZ.y = 0;
        _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);
    }
}
