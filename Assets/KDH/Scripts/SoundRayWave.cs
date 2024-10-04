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
    RaycastHit2D _enemyDetect;

    float _angleStep, _angle;

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
        _angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            if (!isPositionFixed[i])
            {
                _angle = _angleStep * i * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radius, LayerMask.GetMask("Wall") | LayerMask.GetMask("Glass"));
                if (_isPlayerWave)
                {
                    _enemyDetect = Physics2D.Raycast(transform.position, direction, radius, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ammo"));
                    if (_enemyDetect.collider != null && !_contactedEnemy.Contains(_enemyDetect.collider.gameObject)) {
                        _contactedEnemy.Add(_enemyDetect.collider.gameObject);
                        if (_enemyDetect.collider.CompareTag("Enemy")) _enemyDetect.collider.GetComponent<EnemyMovement>().SpawnSprite();
                        else _enemyDetect.collider.GetComponent<SeeByWave>().StartFadeOut();
                    }
                }

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        hit.collider.GetComponent<OutlineColorController>().LookAtWave(transform.position);
                        hit.collider.GetComponent<OutlineColorController>().ShowOutline();
                    }
                    wavePositions[i] = transform.InverseTransformPoint(hit.point);
                    isPositionFixed[i] = true;
                }
                else
                {
                    wavePositions[i] = transform.InverseTransformPoint(transform.position + (Vector3)direction * radius);
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