using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundRayWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;

    [HideInInspector]
    public Color WaveColor;
    public WaveManager waveManager;

    private LineRenderer lineRenderer;
    private Vector3[] wavePositions;
    private float waveExistTime = 0f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments];
        lineRenderer.startColor = lineRenderer.endColor = Color.white;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        radius += growSpeed * Time.fixedDeltaTime;
        SpreadRay();
    }

    void SpreadRay()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), radius, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                wavePositions[i] = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                wavePositions[i] = transform.InverseTransformPoint(transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
            }
        }
        lineRenderer.SetPositions(wavePositions);
    }
}
