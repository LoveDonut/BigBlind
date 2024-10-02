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
    private bool[] _isPositionFixed;
    private List<GameObject> _contactedEnemies = new List<GameObject>();
    private float waveExistTime = 0f;
    float t_Destroy = 0, alpha;
    public float Destroy_Time = 1f;
    public bool _isPlayerWave;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments];
        _isPositionFixed = new bool[segments]; 
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
            if (!_isPositionFixed[i]) 
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), radius, LayerMask.GetMask("Wall") | LayerMask.GetMask("Glass") | (_isPlayerWave ? LayerMask.GetMask("Enemy") : 1 << 1));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Enemy") && _isPlayerWave)
                    {
                        hit.collider.GetComponent<OutlineColorController>().ShowOutline();
                    }
                    else
                    {
                        if (hit.collider.CompareTag("Obstacle")) hit.collider.GetComponent<OutlineColorController>().ShowOutline();
                        wavePositions[i] = transform.InverseTransformPoint(hit.point);
                        _isPositionFixed[i] = true;
                        continue;
                    }
                }
                wavePositions[i] = transform.InverseTransformPoint(transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
            }
        }
        lineRenderer.SetPositions(wavePositions);
    }

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));
        Color waveColor = new(WaveColor.r, WaveColor.g, WaveColor.b, alpha);
        lineRenderer.startColor = waveColor;
        lineRenderer.endColor = waveColor;
        if (alpha <= 0) Destroy(gameObject);

    }
}