using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld _instance;
    [SerializeField] private LayerMask _mousePlaneLayermask;

    void Awake()
    {
        _instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _instance._mousePlaneLayermask);
        return hit.point;
    }

    public static Vector3 GetPositionOnlyHitVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        RaycastHit[] raycastHitsArray =
        Physics.RaycastAll(ray, float.MaxValue, _instance._mousePlaneLayermask);
        //sort by distance of the objects
        System.Array.Sort(raycastHitsArray,(RaycastHit raycastHitA, RaycastHit raycastHitB) =>
        {
           return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance);
        });
        foreach (RaycastHit raycastHit in raycastHitsArray)
        {
            if (raycastHit.transform.TryGetComponent(out Renderer renderer))
            {
                if (renderer.enabled)
                {
                    return raycastHit.point;
                }
            }
        }
        return Vector3.zero;
    }
}
