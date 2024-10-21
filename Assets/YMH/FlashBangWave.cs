using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashBangWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;
    public Color WaveColor;
    private LineRenderer lineRenderer;
    private Vector3[] wavePositions;
    private bool[] isPositionFixed;
    private float waveExistTime = 0f;
    float t_Destroy = 0, alpha;
    public float Destroy_Time = 1f;
    [SerializeField] bool _isPlayerWave;
    List<GameObject> _contactedEnemy = new List<GameObject>();
    RaycastHit2D _enemyDetect;
    float angleStep, angle;


    [SerializeField] float maxRadius = 10;
    Vector2 originPos;


    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void Awake()
    {
        originPos = transform.position;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments];

        for (int i = 0; i < wavePositions.Length; i++)
        {
            wavePositions[i] = originPos;
        }

        isPositionFixed = new bool[segments];

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;
    }

    public void InitWave()
    {
        lineRenderer.startColor = WaveColor;
        lineRenderer.endColor = WaveColor;
    }

    void FixedUpdate()
    {
        radius += growSpeed * Time.fixedDeltaTime;
        radius = Mathf.Min(radius, maxRadius);  // 추가: radius가 maxRadius를 초과하지 않도록 제한
        SpreadRay();
        UpdateWaveColor();
        UpdateMesh();
    }

    void SpreadRay()
    {
        angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            if (!isPositionFixed[i])
            {
                angle = angleStep * i * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radius, LayerMask.GetMask("Wall", "Glass", "Box", "Shield"));

                if (hit.collider != null)
                {
                    wavePositions[i] = transform.InverseTransformPoint(hit.point);
                    isPositionFixed[i] = true;
                }
                else
                {
                    Vector2 newPosition = (Vector2)transform.position + direction * radius;
                    Vector2 clampedPosition = Vector2.ClampMagnitude(newPosition - (Vector2)transform.position, maxRadius) + (Vector2)transform.position;
                    wavePositions[i] = transform.InverseTransformPoint(clampedPosition);
                    isPositionFixed[i] = Vector2.Distance(originPos, wavePositions[i]) >= maxRadius;
                }
            }
        }
        lineRenderer.SetPositions(wavePositions);
    }

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        alpha = WaveColor.a * (1 - (t_Destroy / Destroy_Time));
        Color waveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, alpha);
        if (alpha <= 0) Destroy(gameObject);
        lineRenderer.startColor = waveColor;
        lineRenderer.endColor = waveColor;
        meshRenderer.material.color = new Color(1f, 1f, 1f, alpha);
    }

    void UpdateMesh()
    {
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // 중심점

        for (int i = 0; i < segments; i++)
        {
            vertices[i + 1] = wavePositions[i];

            if (i < segments - 1)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            else
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = 1;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}