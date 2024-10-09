using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshRenderer2DSorter : MonoBehaviour
{
    public string sortingLayerName = "Default";
    public int orderInLayer = 0;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateSortingLayer();
    }

    private void UpdateSortingLayer()
    {
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = orderInLayer;
    }

    private void OnValidate()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        UpdateSortingLayer();
    }

    public void SetSortingLayer(string layerName, int order)
    {
        sortingLayerName = layerName;
        orderInLayer = order;
        UpdateSortingLayer();
    }
}