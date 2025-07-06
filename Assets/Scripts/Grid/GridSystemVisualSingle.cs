using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject _selected;

    public void Show(Material material)
    {
        _meshRenderer.material = material;
        _meshRenderer.enabled = true;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }

    public void ShowSelected(bool show)
    {
        _selected.SetActive(show);
    }
}
