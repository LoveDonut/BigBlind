using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer), typeof(CircleCollider2D))]
public class SoundWave : MonoBehaviour
{
    public int segments = 100;
    public float growSpeed = 0.5f;
    private LineRenderer lineRenderer;
    private CircleCollider2D circleCollider;
    private float radius = 0.5f;
    private Dictionary<int, Vector2> fixedSegments = new Dictionary<int, Vector2>();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;
        DrawCircle();
    }

    private void Update()
    {
        radius += growSpeed * Time.deltaTime;
        circleCollider.radius = radius;
        DrawCircle();
        DetectCollision();
    }

    private void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            if (fixedSegments.TryGetValue(i, out Vector2 fixedPosition))
            {
                lineRenderer.SetPosition(i, fixedPosition);
            }
            else
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                Vector2 newPosition = transform.TransformPoint(new Vector2(x, y));
                lineRenderer.SetPosition(i, newPosition);
            }
            angle += 360f / segments;
        }
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

            if (Vector2.Distance(point, closestPoint) < 0.01f)
            {
                fixedSegments[i] = closestPoint;
            }
        }
    }
}