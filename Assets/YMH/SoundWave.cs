using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class SoundWave : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;


    [Header("웨이브 프레임")]
    [SerializeField] float Frame = 200;

    [HideInInspector]
    public Color WaveColor;
    public WaveManager waveManager;

    private LineRenderer lineRenderer;
    private Dictionary<int, Vector2> fixedSegments = new Dictionary<int, Vector2>();
    private float t_Destroy = 0f;
    private Vector3[] positions;
    private Vector2[] colliderPoints;
    float alpha, angleStep, angle;
    int i;
    int myFrame;
    //프레임 보정값
    float frameRate;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        positions = new Vector3[segments + 1];
        colliderPoints = new Vector2[segments + 1];

        myFrame = Application.targetFrameRate == -1 ? 100 : Application.targetFrameRate;
        frameRate = 10 * Mathf.Cos(Mathf.PI / (2 * Mathf.Clamp01((myFrame / 200))));
        if (frameRate <= 1 || float.IsNaN(frameRate)) frameRate = 1;
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
        alpha = WaveColor.a - (t_Destroy / waveManager.Destroy_Time);
        Color waveColor = new(WaveColor.r, WaveColor.g, WaveColor.b, alpha);
        lineRenderer.startColor = lineRenderer.endColor = waveColor;
    }

    private void DrawCircle()
    {
        angleStep = 360f / segments;
        for (i = 0; i <= segments; i++)
        {
            if (fixedSegments.TryGetValue(i, out Vector2 fixedPosition))
            {
                positions[i] = fixedPosition;
            }
            else
            {
                angle = i * angleStep * Mathf.Deg2Rad;
                positions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            colliderPoints[i] = positions[i];
        }
        lineRenderer.SetPositions(positions);

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
        for (i = 0; i <= segments; i++)
        {
            if (fixedSegments.ContainsKey(i)) continue;
            Vector2 worldPoint = transform.TransformPoint(positions[i]);
            Vector2 closestPoint = wallCollider.ClosestPoint(worldPoint);
            if (Vector2.Distance(worldPoint, closestPoint) <= 0.1f * frameRate)
            {
                Vector2 localPoint = transform.InverseTransformPoint(closestPoint);
                fixedSegments[i] = localPoint;
                positions[i] = localPoint;
            }
        }
    }
}