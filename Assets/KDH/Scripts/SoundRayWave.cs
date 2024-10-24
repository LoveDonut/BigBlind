using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SoundRayWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float _detectRadius = .2f;


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

    private readonly Collider2D[] overlapResults = new Collider2D[5];

    [SerializeField] float raycastCheckInterval = 0.05f;

    List<GameObject> detectedObj = new List<GameObject>();

    [SerializeField] LayerMask waveLayerMask;
    [SerializeField] LayerMask itemMask;
    [SerializeField] LayerMask wallMask;

    private float angleStep;
    private float currentAngle;

    // 각 라인 렌더러의 중간점 위치와 고정 상태를 저장
    private Vector3[] middlePoints;
    private bool[] isMiddlePointFixed;

    float alpha;

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
        middlePoints = new Vector3[segments];
        isMiddlePointFixed = new bool[segments];
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
            LineRenderer lr = LineRendererPool.Instance.GetLineRenderer();
            lr.transform.SetParent(transform);
            SetupLineRendererProperties(lr);
            lineRenderers[i] = lr;
        }
    }

    private void OnDestroy()
    {
        if (_isPlayerWave && !isWaveEffect && lineRenderers != null)
        {
            foreach (var lr in lineRenderers)
            {
                if (lr != null)
                {
                    LineRendererPool.Instance.ReturnToPool(lr);
                }
            }
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
        lr.positionCount = 3;
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

    private void Start()
    {
        StartCoroutine(SpreadRay());
    }

    void FixedUpdate()
    {
        radius += growSpeed * Time.fixedDeltaTime;
    }

    IEnumerator SpreadRay()
    {
        do
        {
            Vector3 position = transform.position;
            currentAngle = 0f;

            for (int i = 0; i < segments; i++)
            {
                rayDirection.Set(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

                if (!isPositionFixed[i])
                {
                    ProcessSegment(i, position);
                }

                // 중점에 대한 레이캐스트 처리
                if (!isMiddlePointFixed[i])
                {
                    ProcessMiddlePoint(i, position);
                }

                currentAngle += angleStep;
            }

            wavePositions[segments] = wavePositions[0];
            UpdateLineRendererPositions();
            UpdateWaveColor();
            yield return new WaitForSeconds(raycastCheckInterval);
        }
        while (true);
    }

    private void ProcessMiddlePoint(int index, Vector3 position)
    {
        // 중간 각도 계산

        int nextIndex = (index + 1) % segments;

        float middleAngle = currentAngle + (angleStep * 0.5f);
        Vector3 midPoint = (wavePositions[index] + wavePositions[nextIndex]) * 0.5f;
        Vector2 middleDirection = new Vector2(Mathf.Cos(middleAngle), Mathf.Sin(middleAngle));

        RaycastHit2D hit = Physics2D.Raycast(position, middleDirection, radius, waveLayerMask);

        if (hit.collider != null && !isWaveEffect)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                middlePoints[index] = hit.point;
                isMiddlePointFixed[index] = true;
            }
            else
            {
                middlePoints[index] = midPoint;
            }
        }
        else
        {
            middlePoints[index] = (Vector3)(middleDirection * radius) + position;
        }
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
            int nextIndex = (i + 1) % segments;
            if (lineRenderers[i].material == _waveMaterials[1]) continue;

            lineRenderers[i].SetPosition(0, wavePositions[i]);
            lineRenderers[i].SetPosition(1, middlePoints[i]);
            lineRenderers[i].SetPosition(2, wavePositions[nextIndex]);

            bool allPointsFixed = isPositionFixed[i] && isMiddlePointFixed[i] && isPositionFixed[nextIndex];

            float distancePole = Vector2.Distance(wavePositions[i], wavePositions[nextIndex]);
            float distanceStart = Vector2.Distance(wavePositions[i], middlePoints[i]);
            float distanceEnd = Vector2.Distance(middlePoints[i], wavePositions[nextIndex]);

            bool middleWallcheck;
            if(distanceStart > distanceEnd)
            {
                middleWallcheck = Physics2D.OverlapCircle((wavePositions[i] + middlePoints[i]) / 2, _detectRadius,  LayerMask.GetMask("Wall")) ;
            }
            else
            {
                middleWallcheck = Physics2D.OverlapCircle((wavePositions[nextIndex] + middlePoints[i]) / 2, _detectRadius,  LayerMask.GetMask("Wall"));
            }

 
            bool isLong = Mathf.Max(distanceStart, distanceEnd) >= distancePole || !middleWallcheck;


            if (allPointsFixed && !isLong)
            {
                lineRenderers[i].material = _waveMaterials[1];
                lineRenderers[i].material.SetColor("_BaseColor", new Color(WaveColor.r, WaveColor.g, WaveColor.b, alpha));
            }
            else
            {
                lineRenderers[i].material = _waveMaterials[0];
            }
        }
    }

    void UpdateWaveColor()
    {
        t_Destroy += raycastCheckInterval;
        alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));

        if (_isPlayerWave && !isWaveEffect)
        {
            foreach (var lr in lineRenderers)
            {
                lr.material.SetFloat("_Alpha", alpha);
            }
        }
        else
        {
            singleLineRenderer.material.SetFloat("_Alpha", alpha);
        }

        if (alpha <= 0) Destroy(gameObject);
    }
}