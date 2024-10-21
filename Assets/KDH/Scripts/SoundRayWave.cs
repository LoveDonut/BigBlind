using System.Collections.Generic;
using UnityEngine;

public class SoundRayWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;
    public Color WaveColor;
    private LineRenderer[] lineRenderers;
    private LineRenderer singleLineRenderer;
    private Vector3[] wavePositions;
    private bool[] isPositionFixed;
    private float t_Destroy = 0;
    public float Destroy_Time = 1f;
    [SerializeField] bool _isPlayerWave;
    private Vector2 rayDirection;

    [SerializeField] float linewidth = .2f;
    [SerializeField] float maxSegmentDistance = 1f;

    Material[] _waveMaterials;

    public bool isWaveEffect = false;

    private float[] detectRadius;

    [HideInInspector]
    public bool isCannonWave = false;

    List<GameObject> detectedObj = new List<GameObject>();

    [SerializeField] LayerMask waveLayerMask;
    [SerializeField] LayerMask itemMask;
    [SerializeField] LayerMask wallMask;

    private float angleStep;
    private float currentAngle;

    void Awake()
    {
        InitializeArrays();
        SetupLineRenderers();
        angleStep = 360f / segments * Mathf.Deg2Rad;
        rayDirection = new Vector2(1, 0);
    }

    private void InitializeArrays()
    {
        wavePositions = new Vector3[segments + 1];
        isPositionFixed = new bool[segments];
        detectRadius = new float[segments];
    }

    private void SetupLineRenderers()
    {
        _waveMaterials = GetComponent<LineRenderer>().materials;

        if (_isPlayerWave && !isWaveEffect)
        {
            SetupMultipleLineRenderers();
        }
        else
        {
            SetupSingleLineRenderer();
        }
    }

    private void SetupMultipleLineRenderers()
    {
        lineRenderers = new LineRenderer[segments];
        for (int i = 0; i < segments; i++)
        {
            GameObject lineObj = new GameObject($"LineRenderer_{i}");
            lineObj.transform.SetParent(transform);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            SetupLineRendererProperties(lr);
            lineRenderers[i] = lr;
        }
    }

    private void SetupSingleLineRenderer()
    {
        singleLineRenderer = GetComponent<LineRenderer>();
        singleLineRenderer.positionCount = segments + 1;
        singleLineRenderer.useWorldSpace = true;
        singleLineRenderer.startWidth = singleLineRenderer.endWidth = 0.1f;
        singleLineRenderer.loop = true;
    }

    private void SetupLineRendererProperties(LineRenderer lr)
    {
        lr.startWidth = linewidth;
        lr.material = _waveMaterials[0];
        _waveMaterials[1].SetColor("Tint", WaveColor);
        lr.positionCount = 2;
        lr.useWorldSpace = true;
    }

    public void InitWave()
    {
        if (_isPlayerWave && !isWaveEffect)
        {
            foreach (var lr in lineRenderers)
            {
                lr.startColor = lr.endColor = WaveColor;
            }
        }
        else
        {
            singleLineRenderer.startColor = singleLineRenderer.endColor = WaveColor;
        }
    }

    void FixedUpdate()
    {
        radius += growSpeed * Time.fixedDeltaTime;
        SpreadRay();
        UpdateWaveColor();
    }

    void SpreadRay()
    {
        Vector3 position = transform.position;
        currentAngle = 0f;

        for (int i = 0; i < segments; i++)
        {
            if (!isPositionFixed[i])
            {
                rayDirection.Set(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
                ProcessSegment(i, position);
            }
            currentAngle += angleStep;
        }

        wavePositions[segments] = wavePositions[0];
        UpdateLineRendererPositions();
    }

    private void ProcessSegment(int index, Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, rayDirection, radius, waveLayerMask);

        if (_isPlayerWave && !isWaveEffect)
        {
            ProcessPlayerWave(position);
        }

        if (hit.collider != null && !isWaveEffect)
        {
            ProcessCollision(hit, index);
        }
        else
        {
            wavePositions[index] = (Vector3)(rayDirection * radius) + position;
        }
    }

    private void ProcessPlayerWave(Vector3 position)
    {
        RaycastHit2D enemyHit = Physics2D.Raycast(position, rayDirection, radius, itemMask);
        if (enemyHit.collider != null)
        {
            enemyHit.collider.GetComponent<SeeByWave>()?.StartFadeOut();
        }
    }

    private void ProcessCollision(RaycastHit2D hit, int index)
    {
        if (hit.collider.CompareTag("Obstacle") && !detectedObj.Contains(hit.collider.gameObject))
        {
            ProcessObstacleCollision(hit);
            if (!isCannonWave) return;
        }

        if (hit.collider.CompareTag("Wall"))
        {
            wavePositions[index] = hit.point;
            isPositionFixed[index] = true;
            if (_isPlayerWave) detectRadius[index] = radius;
        }
        else
        {
            wavePositions[index] = (Vector3)(rayDirection * radius) + transform.position;
        }
    }

    private void ProcessObstacleCollision(RaycastHit2D hit)
    {
        detectedObj.Add(hit.collider.gameObject);
        OutlineColorController outlineController = hit.collider.GetComponent<OutlineColorController>();
        outlineController.LookAtWave(transform.position);
        outlineController.ShowOutline();
    }

    private void UpdateLineRendererPositions()
    {
        if (_isPlayerWave && !isWaveEffect)
        {
            UpdateMultipleLineRenderers();
        }
        else
        {
            singleLineRenderer.SetPositions(wavePositions);
        }
    }

    private void UpdateMultipleLineRenderers()
    {
        for (int i = 0; i < segments; i++)
        {
            if (lineRenderers[i].material == _waveMaterials[1]) continue;

            int nextIndex = (i + 1) % segments;
            UpdateLineRendererSegment(i, nextIndex);
        }
    }

    private void UpdateLineRendererSegment(int currentIndex, int nextIndex)
    {
        bool bothSegmentsFixed = isPositionFixed[currentIndex] && isPositionFixed[nextIndex];
        float segmentDistance = Vector3.Distance(wavePositions[currentIndex], wavePositions[nextIndex]);

        var wallCheck = Physics2D.OverlapCircle(
            (wavePositions[currentIndex] + wavePositions[nextIndex]) * 0.5f,
            Mathf.Min(detectRadius[currentIndex], detectRadius[nextIndex]) * maxSegmentDistance,
            wallMask
        );

        bool shouldChangeMaterial = bothSegmentsFixed &&
            segmentDistance <= Mathf.Min(detectRadius[currentIndex], detectRadius[nextIndex]) * 0.8f &&
            wallCheck != null;

        lineRenderers[currentIndex].material = shouldChangeMaterial ? _waveMaterials[1] : _waveMaterials[0];
        lineRenderers[currentIndex].SetPosition(0, wavePositions[currentIndex]);
        lineRenderers[currentIndex].SetPosition(1, wavePositions[nextIndex]);
    }

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        float alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));
        Color waveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, alpha);

        if (_isPlayerWave && !isWaveEffect)
        {
            foreach (var lr in lineRenderers)
            {
                lr.startColor = lr.endColor = waveColor;
            }
        }
        else
        {
            singleLineRenderer.startColor = singleLineRenderer.endColor = waveColor;
        }

        if (alpha <= 0) Destroy(gameObject);
    }
}