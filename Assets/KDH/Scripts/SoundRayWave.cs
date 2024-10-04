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
    private bool[] isPositionFixed;  // 각 위치가 고정되었는지 추적하는 배열
    private float waveExistTime = 0f;
    float t_Destroy = 0, alpha;
    public float Destroy_Time = 1f;

    [SerializeField] bool _isPlayerWave;

    List<GameObject> _contactedEnemy = new List<GameObject>();
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments];
        isPositionFixed = new bool[segments];  // 초기화
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
            if (!isPositionFixed[i])  // 이 위치가 아직 고정되지 않았다면
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), radius, LayerMask.GetMask("Wall") | LayerMask.GetMask("Glass") | (_isPlayerWave ? LayerMask.GetMask("Enemy") : 1 << 1));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        wavePositions[i] = transform.InverseTransformPoint(transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
                        if (_contactedEnemy.Find(x => x == hit.collider.gameObject)) continue;
                        _contactedEnemy.Add(hit.collider.gameObject);
                        hit.collider.GetComponent<EnemyMovement>().SpawnSprite();
                        continue;
                    }
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        hit.collider.GetComponent<OutlineColorController>().LookAtWave(transform.position);
                        hit.collider.GetComponent<OutlineColorController>().ShowOutline();
                    }
                    wavePositions[i] = transform.InverseTransformPoint(hit.point);
                    isPositionFixed[i] = true;  // 위치를 고정
                }
                else
                {
                    wavePositions[i] = transform.InverseTransformPoint(transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
                }
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