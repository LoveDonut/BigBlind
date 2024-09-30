using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundRayWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;

    public Color WaveColor;

    private LineRenderer lineRenderer;
    private Vector3[] wavePositions;
    private float waveExistTime = 0f;

    float t_Destroy = 0, alpha;

    public float Destroy_Time = 1f;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments];
    }

    public void InitWave()
    {
        lineRenderer.startColor = WaveColor;
        lineRenderer.endColor = WaveColor;
    }

    void FixedUpdate()
    {
        radius += growSpeed * Time.fixedDeltaTime;
        SpreadRay();
        UpdateWaveColor();
    }


    void SpreadRay()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), radius, LayerMask.GetMask("Wall") | LayerMask.GetMask("Glass"));
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

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        radius += growSpeed * Time.fixedDeltaTime;
        alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));
        Color waveColor = new(WaveColor.r, WaveColor.g, WaveColor.b, alpha);
        lineRenderer.startColor  = waveColor;
        lineRenderer.endColor = waveColor;

        if (alpha <= 0) Destroy(gameObject);
    }
}
