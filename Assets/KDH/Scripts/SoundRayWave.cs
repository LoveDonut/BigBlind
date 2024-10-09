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
    private Vector2 rayDirection = Vector2.zero;

    [SerializeField] float linewidth = .2f;
    [SerializeField] float maxSegmentDistance = 1f; // New variable for max distance between segments


    Material[] _waveMaterials;

    void Awake()
    {
        wavePositions = new Vector3[segments + 1]; // +1 to close the loop
        isPositionFixed = new bool[segments];
        _waveMaterials = GetComponent<LineRenderer>().materials;
        if (_isPlayerWave)
        {
            lineRenderers = new LineRenderer[segments];
            for (int i = 0; i < segments; i++)
            {
                GameObject lineObj = new GameObject($"LineRenderer_{i}");
                lineObj.transform.SetParent(transform);
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();

                lr.startWidth = linewidth;
                lr.material = _waveMaterials[0];
                lr.positionCount = 2;
                lr.useWorldSpace = true;
                lineRenderers[i] = lr;
            }
        }
        else
        {
            singleLineRenderer = GetComponent<LineRenderer>();
            singleLineRenderer.positionCount = segments + 1;
            singleLineRenderer.useWorldSpace = true;
            singleLineRenderer.startWidth = singleLineRenderer.endWidth = 0.1f;
            singleLineRenderer.loop = true;
        }
    }

    public void InitWave()
    {
        if (_isPlayerWave)
        {
            for (int i = 0; i < segments; i++)
            {
                lineRenderers[i].startColor = lineRenderers[i].endColor = WaveColor;
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
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            rayDirection.Set(Mathf.Cos(angle), Mathf.Sin(angle));

            if (!isPositionFixed[i])
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, radius, LayerMask.GetMask("Wall", "Glass", "Box"));

                if (_isPlayerWave)
                {
                    RaycastHit2D enemyHit = Physics2D.Raycast(transform.position, rayDirection, radius, LayerMask.GetMask("Item"));
                    if (enemyHit.collider != null)
                    {
                        enemyHit.collider.GetComponent<SeeByWave>()?.StartFadeOut();
                    }
                }

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        OutlineColorController outlineController = hit.collider.GetComponent<OutlineColorController>();
                        outlineController.LookAtWave(transform.position);
                        outlineController.ShowOutline();
                    }
                    wavePositions[i] = hit.point;
                    isPositionFixed[i] = true;
                }
                else
                {
                    wavePositions[i] = (Vector3)rayDirection * radius + transform.position;
                }
            }
        }

        // Close the loop
        wavePositions[segments] = wavePositions[0];

        // Update LineRenderer positions
        if (_isPlayerWave)
        {
            for (int i = 0; i < segments; i++)
            {
                int nextIndex = (i + 1) % segments;
                bool bothSegmentsFixed = isPositionFixed[i] && isPositionFixed[nextIndex];
                float segmentDistance = Vector3.Distance(wavePositions[i], wavePositions[nextIndex]);

                bool shouldChangeMaterial = bothSegmentsFixed && segmentDistance <= maxSegmentDistance;
                lineRenderers[i].material = shouldChangeMaterial ? _waveMaterials[1] : _waveMaterials[0];

                lineRenderers[i].SetPosition(0, wavePositions[i]);
                lineRenderers[i].SetPosition(1, wavePositions[nextIndex]);
            }
        }
        else
        {
            singleLineRenderer.SetPositions(wavePositions);
        }
    }

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        float alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));
        Color waveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, alpha);

        if (_isPlayerWave)
        {
            for (int i = 0; i < segments; i++)
            {
                lineRenderers[i].startColor = lineRenderers[i].endColor = waveColor;
            }
        }
        else
        {
            singleLineRenderer.startColor = singleLineRenderer.endColor = waveColor;
        }

        if (alpha <= 0) Destroy(gameObject);
    }
}