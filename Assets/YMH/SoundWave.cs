using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class SoundWave : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;

    public WaveManager waveManager;

    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;
    private Dictionary<int, Vector2> fixedSegments = new Dictionary<int, Vector2>();
    private float t_Destroy = 0f;
    private Vector3[] positions;
    private Vector2[] colliderPoints;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;

        positions = new Vector3[segments + 1];
        colliderPoints = new Vector2[segments + 1];
    }

    private void Update()
    {
        UpdateWaveProperties();
        DrawCircle();
        DetectCollision();
    }

    private void UpdateWaveProperties()
    {
        t_Destroy += Time.deltaTime;
        radius += growSpeed * Time.deltaTime;
        float alpha = 1 - (t_Destroy / waveManager.Destroy_Time);
        Color waveColor = new Color(0.5f, 0.5f, 0.5f, waveManager.Destroy_Time - t_Destroy);
        lineRenderer.startColor = lineRenderer.endColor = waveColor;
    }

    private void DrawCircle()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            if (fixedSegments.TryGetValue(i, out Vector2 fixedPosition))
            {
                positions[i] = fixedPosition;
            }
            else
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                positions[i] = transform.TransformPoint(pos);
            }
            colliderPoints[i] = positions[i];
        }

        lineRenderer.SetPositions(positions);
        polygonCollider.SetPath(0, colliderPoints);
    }

    private void DetectCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                DeformCircle(collider);
            }
        }
    }

    private void DeformCircle(Collider2D wallCollider)
    {
        for (int i = 0; i <= segments; i++)
        {
            if (fixedSegments.ContainsKey(i)) continue;
            Vector2 point = positions[i];
            Vector2 closestPoint = wallCollider.ClosestPoint(point);
            if (Vector2.Distance(point, closestPoint) < 0.01f)
            {
                fixedSegments[i] = closestPoint;
                positions[i] = closestPoint;
            }
        }
    }
}