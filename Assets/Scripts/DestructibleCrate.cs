using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField] private Transform _crateDesstroyed;
    public static event EventHandler OnAnyDestroyed;
    private GridPosition _gridPosition;

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }
    public void Damage()
    {
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
        Transform crateDestroyedTransform = Instantiate(_crateDesstroyed, transform.position, transform.rotation);
        ApplyExplosionChildren(crateDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);
    }

    public GridPosition GetGridPosition() => _gridPosition;

    private void ApplyExplosionChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            //Recursive call to apply explosion force to all child bones
            ApplyExplosionChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
