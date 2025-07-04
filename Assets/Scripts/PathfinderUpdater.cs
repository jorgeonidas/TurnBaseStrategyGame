using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void OnDestroy()
    {
        DestructibleCrate.OnAnyDestroyed -= DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        BoxCollider col = destructibleCrate.GetComponent<BoxCollider>();
        if (col != null)
        {
            Pathfinding.Instance.SetIsWalkableInsideColliderBounds(col, true);
        }
        
    }
}
