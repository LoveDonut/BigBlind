using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveOnBlood : MonoBehaviour
{
    #region references
    [SerializeField] UnityEngine.GameObject _circle; // wave prefab
    #endregion

    #region PrivateVariables
    float _waveInterval;
    Color _backgroundColor;
    Vector2 _circlePosition;
    Coroutine _waveCoroutine;
    #endregion

    private SpriteRenderer circleRenderer;

    void Start()
    {
        circleRenderer = _circle.GetComponent<SpriteRenderer>();

        _backgroundColor = Camera.main.backgroundColor;

        circleRenderer.color = _backgroundColor;

        _waveInterval = GetComponent<WaveManager>().DestroyTime * 2;
    }

    public void WaveOnBlood()
    {
        if (circleRenderer.color.a > 0) return;
        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
        }

        _circlePosition = transform.position;

        _waveCoroutine = StartCoroutine(FadeOutCircle());
    }

    IEnumerator FadeOutCircle()
    {
        float elapsedTime = 0f;

        // alpha is 1 at first
        Color circleColor = _backgroundColor;
        circleColor.a = 1f;
        circleRenderer.color = circleColor;

        // fade out
        while (elapsedTime < _waveInterval)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _waveInterval);
            circleColor.a = alpha;
            circleRenderer.color = circleColor;

            _circle.transform.position = _circlePosition;

            yield return null;
        }
    }
}
