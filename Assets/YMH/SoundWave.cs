using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(LineRenderer))]
public class SoundWave : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private int _segments = 100;
    [SerializeField] private float _growSpeed = 0.5f;
    [SerializeField] private float _radius = 0.5f;


    [Header("웨이브 프레임")]
    [SerializeField] float _frame = 200;

    public bool isCannonWave;

    [HideInInspector]
    public Color WaveColor;
    public WaveManager WaveManager;

    private LineRenderer _lineRenderer;
    private Dictionary<int, Vector2> fixedSegments = new Dictionary<int, Vector2>();
    private float t_Destroy = 0f;
    private Vector3[] positions;
    private Vector2[] colliderPoints;
    float alpha, angleStep, angle;
    int i;
    int myFrame;
    //프레임 보정값
    float frameRate;

    float Destroy_Time;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _segments + 1;
        _lineRenderer.useWorldSpace = false;
        positions = new Vector3[_segments + 1];
        colliderPoints = new Vector2[_segments + 1];
        //myFrame = Application.targetFrameRate == -1 ? 70 : Application.targetFrameRate;
        frameRate = 10 * Mathf.Cos(Mathf.PI / 2.5f);
        print(frameRate);
        //if (frameRate <= 1 || float.IsNaN(frameRate)) frameRate = 1;
    }

    public void Init()
    {
        WaveColor = WaveManager.WaveColor;
        Destroy_Time = isCannonWave ? WaveManager.CannonDestroyTime : WaveManager.DestroyTime;
    }

    private void Update()
    {
        UpdateWaveProperties();
        DrawCircle();
        DetectCollision();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < 1000; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, float.PositiveInfinity, LayerMask.GetMask("Wall"));
        }
    }

    private void UpdateWaveProperties()
    {
        t_Destroy += Time.deltaTime;
        _radius += _growSpeed * Time.deltaTime;
        alpha = WaveColor.a - (t_Destroy / Destroy_Time);
        Color waveColor = new(WaveColor.r, WaveColor.g, WaveColor.b, alpha);
        _lineRenderer.startColor  = waveColor;
        _lineRenderer.endColor = waveColor;
    }

    private void DrawCircle()
    {
        angleStep = 360f / _segments;
        for (i = 0; i <= _segments; i++)
        {
            if (fixedSegments.TryGetValue(i, out Vector2 fixedPosition))
            {
                positions[i] = fixedPosition;
            }
            else
            {
                angle = i * angleStep * Mathf.Deg2Rad;
                positions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
            }
            colliderPoints[i] = positions[i];
        }
        _lineRenderer.SetPositions(positions);

    }

    private void DetectCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _radius);
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
        for (i = 0; i <= _segments; i++)
        {
            if (fixedSegments.ContainsKey(i)) continue;
            Vector2 worldPoint = transform.TransformPoint(positions[i]);
            Bounds bounds = wallCollider.bounds;
            if (bounds.Contains(worldPoint))
            {
                float closestX = (worldPoint.x < bounds.center.x) ? bounds.min.x : bounds.max.x;
                float closestY = (worldPoint.y < bounds.center.y) ? bounds.min.y : bounds.max.y;
                Vector2 closestPoint = new Vector2(closestX, closestY);
                Vector2 localPoint = transform.InverseTransformPoint(closestPoint);
                fixedSegments[i] = localPoint;
                positions[i] = localPoint;
            }
            /*
            Vector2 closestPoint = wallCollider.ClosestPoint(worldPoint);
            if (Vector2.Distance(worldPoint, closestPoint) <= 0.15f * frameRate)
            {
                Vector2 localPoint = worldPoint == closestPoint ? transform.InverseTransformPoint(wallCollider.ClosestPoint(closestPoint - (closestPoint - new Vector2(transform.position.x, transform.position.y)).normalized * 0.6f * frameRate)) : transform.InverseTransformPoint(closestPoint);
                fixedSegments[i] = localPoint;
                positions[i] = localPoint;
            }
            */
        }
    }
}