using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class SoundWave : MonoBehaviour
{
    [Header("기본 설정")]
    public int segments = 100;
    public float growSpeed = 0.5f;
    public float Disappear_Time = .5f;
    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;
    private float radius = 0.5f;
    private Dictionary<int, Vector2> fixedSegments = new Dictionary<int, Vector2>();
    [HideInInspector]
    public WaveManager waveManager;
    float t_Destroy = 0f;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;
        DrawCircle();
    }
    private void Update()
    {
        t_Destroy += Time.deltaTime;
        Color startColor = Color.white;
        Color endColor = Color.black;
        lineRenderer.startColor = Color.Lerp(startColor, endColor, t_Destroy / waveManager.Destroy_Time);
        lineRenderer.endColor = Color.Lerp(startColor, endColor, t_Destroy / waveManager.Destroy_Time);
        radius += growSpeed * Time.deltaTime;
        DrawCircle();
        DetectCollision();
    }
    private void DrawCircle()
    {
        float angle = 0f;
        List<Vector2> colliderPoints = new List<Vector2>(); 

        for (int i = 0; i <= segments; i++)
        {
            if (fixedSegments.TryGetValue(i, out Vector2 fixedPosition))
                lineRenderer.SetPosition(i, fixedPosition);
            else
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                Vector2 newPosition = transform.TransformPoint(new Vector2(x, y));
                lineRenderer.SetPosition(i, newPosition);
            }

            colliderPoints.Add(lineRenderer.GetPosition(i));

            angle += 360f / segments;
        }

        polygonCollider.SetPath(0, colliderPoints.ToArray());
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
            Vector2 point = lineRenderer.GetPosition(i);
            Vector2 closestPoint = wallCollider.ClosestPoint(point);
            if (Vector2.Distance(point, closestPoint) < 0.01f) fixedSegments[i] = closestPoint;
        }
    }
}