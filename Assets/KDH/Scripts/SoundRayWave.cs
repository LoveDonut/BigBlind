using UnityEngine;

public class SoundRayWave : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float growSpeed = 0.5f;
    [SerializeField] private float radius = 0.5f;
    public Color WaveColor;
    public Color FillColor; // 새로 추가: 채우기 색상
    private LineRenderer lineRenderer;
    private MeshFilter meshFilter; // 새로 추가: 메쉬 필터
    private MeshRenderer meshRenderer; // 새로 추가: 메쉬 렌더러
    private Vector3[] wavePositions;
    private bool[] isPositionFixed;
    private float waveExistTime = 0f;
    float t_Destroy = 0, alpha;
    public float Destroy_Time = 1f;
    [SerializeField] bool _isPlayerWave;

    GameObject _waveSilhouette;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1; 
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        wavePositions = new Vector3[segments + 1];
        isPositionFixed = new bool[segments + 1];

        if (_isPlayerWave)
        {
            _waveSilhouette = Instantiate(new GameObject("_playerwaveSilhouette"), transform.position, Quaternion.identity);
            _waveSilhouette.AddComponent<WaveSilhouette>();
            _waveSilhouette.transform.SetParent(GameObject.Find("Silhouettes").transform);
            meshFilter = _waveSilhouette.AddComponent<MeshFilter>();
            meshRenderer = _waveSilhouette.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
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
        UpdateMesh(); // 새로 추가: 메쉬 업데이트
    }

    void SpreadRay()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            if (!isPositionFixed[i])
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radius, LayerMask.GetMask("Wall", "Glass", "Box"));

                if (hit.collider != null)
                {
                    wavePositions[i] = transform.InverseTransformPoint(hit.point);
                    isPositionFixed[i] = true;
                }
                else
                {
                    wavePositions[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
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
        lineRenderer.startColor = waveColor;
        lineRenderer.endColor = waveColor;
        if (alpha <= 0) Destroy(gameObject);
    }

    void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // 중심점
        for (int i = 0; i <= segments; i++)
        {
            vertices[i + 1] = wavePositions[i];
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        if(meshFilter != null) meshFilter.mesh = mesh;
    }
}