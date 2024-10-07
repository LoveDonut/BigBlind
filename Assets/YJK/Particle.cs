using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    #region References
    [Header("References")]
    public GameObject LineEnd;
    #endregion

    LineRenderer _lineRenderer;
    [HideInInspector] public float DestroyTime = 5f;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Invoke("DelayedDestroy", DestroyTime);
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, LineEnd.transform.position);
        if (Vector2.Distance(transform.position, LineEnd.transform.position) > 0.25f) _lineRenderer.enabled = false;
        else _lineRenderer.enabled = true;
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
