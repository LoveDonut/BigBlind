using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class OutlineColorController : MonoBehaviour
{
    public SpriteRenderer _spriteRenderer;
    [SerializeField] Material _instancedMaterial;
    [SerializeField] float _duration;
    [SerializeField] Color _color;
    [SerializeField] float _offset;
    bool _isPlaying;

    float elapsedTime, alpha, angle;
    private void OnEnable()
    {
        SetupMaterial();
    }

    private void OnDisable()
    {
        CleanupMaterial();
    }

    private void SetupMaterial()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (_spriteRenderer != null && _spriteRenderer.sharedMaterial != null)
        {
            _instancedMaterial = new Material(_spriteRenderer.sharedMaterial);
            _spriteRenderer.material = _instancedMaterial;
            _instancedMaterial.SetFloat("_OutlineMode", 1);
            _instancedMaterial.SetColor("_GradientOutline1", new Color(1, 1, 1, 0));
        }
    }

    private void CleanupMaterial()
    {
        if (_spriteRenderer != null && _instancedMaterial != null)
        {
            _spriteRenderer.material = _spriteRenderer.sharedMaterial;
        }
    }

    public void ShowOutline()
    {
        if (_isPlaying || _instancedMaterial == null) return;
        StartCoroutine(FadeOutline(_color));
    }

    private IEnumerator FadeOutline(Color color)
    {
        _isPlaying = true;
        float fullDuration = _duration;
        float startTime = Time.time;

        while (Time.time - startTime < fullDuration)
        {
            float t = (Time.time - startTime) / fullDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            if (t < .25)
            {
                // 첫 번째 단계: Outline1 페이드 아웃
                float alpha1 = Mathf.Lerp(1f, 0f, smoothT * 3);
                _instancedMaterial.SetColor("_GradientOutline1", new Color(color.r, color.g, color.b, alpha1));
            }
            else if (t < .5)
            {
                // 두 번째 단계: Outline1 과 Outline2 페이드 인 완료
                float alpha2 = Mathf.Lerp(0f, 1f, (smoothT - .25f) * 3);
                _instancedMaterial.SetColor("_GradientOutline1", new Color(color.r, color.g, color.b, 1 - alpha2));
                _instancedMaterial.SetColor("_GradientOutline2", new Color(color.r, color.g, color.b, alpha2));
            }
            else if(t < .75f)
            {
                // 세 번쨰 단계 : Outline1 페이드 아웃
                float alpha3 = Mathf.Lerp(0f, 1f, (smoothT - .5f) * 3);
                _instancedMaterial.SetColor("_GradientOutline1", new Color(color.r, color.g, color.b, 1 - alpha3));

            }
            else
            {
                // 세 번째 단계: Outline2 페이드 아웃
                float alpha3 = Mathf.Lerp(1f, 0f, (smoothT - .75f) * 3);
                _instancedMaterial.SetColor("_GradientOutline2", new Color(color.r, color.g, color.b, alpha3));
            }

            yield return null;
        }

        // 최종 상태 설정
        _instancedMaterial.SetColor("_GradientOutline1", new Color(color.r, color.g, color.b, 0));
        _instancedMaterial.SetColor("_GradientOutline2", new Color(color.r, color.g, color.b, 0));

        _isPlaying = false;
    }

    public void LookAtWave(Vector3 wavePos)
    {
        if (_isPlaying || _instancedMaterial == null) return;
        angle = Mathf.Atan2(wavePos.y - transform.position.y, wavePos.x - transform.position.x) * Mathf.Rad2Deg;
        _instancedMaterial.SetFloat("_Angle", angle - _offset);
    }

    public void CopySettingsFrom(OutlineColorController other)
    {
        if (other._instancedMaterial != null && this._instancedMaterial != null)
        {
            this._instancedMaterial.CopyPropertiesFromMaterial(other._instancedMaterial);
        }
    }
}