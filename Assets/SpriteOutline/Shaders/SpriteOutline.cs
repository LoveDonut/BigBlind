using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class OutlineColorController : MonoBehaviour
{
    public SpriteRenderer _spriteRenderer;
    private Material _instancedMaterial;
    [SerializeField] float _duration;
    [SerializeField] Color _color;
    bool _isPlaying;

    float elapsedTime, alpha;
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
        }
    }

    private void CleanupMaterial()
    {
        if (_spriteRenderer != null && _instancedMaterial != null)
        {
            _spriteRenderer.material = _spriteRenderer.sharedMaterial;

            if (Application.isPlaying)
            {
                Destroy(_instancedMaterial);
            }
            else
            {
                DestroyImmediate(_instancedMaterial);
            }
        }
    }

    public void ShowOutline()
    {
        if (_isPlaying) return;
        StartCoroutine(FadeOutline(_color));
    }

    private IEnumerator FadeOutline(Color color)
    {
        _isPlaying = true;
        elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += .01f;
            alpha = Mathf.Lerp(1f, 0f, elapsedTime / _duration);
            _instancedMaterial.SetColor("_SolidOutline", new Color(color.r, color.g, color.b, alpha));
            yield return new WaitForSeconds(.01f);
        }
        _isPlaying = false;
    }

    public void CopySettingsFrom(OutlineColorController other)
    {
        if (other._instancedMaterial != null && this._instancedMaterial != null)
        {
            this._instancedMaterial.CopyPropertiesFromMaterial(other._instancedMaterial);
        }
    }
}