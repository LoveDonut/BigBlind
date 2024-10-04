using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShadow : MonoBehaviour
{
    [SerializeField] float _duration = 1;
    bool _isPlaying;
    [SerializeField] Color _color;

    private void Start()
    {
        _color = GetComponent<SpriteRenderer>().color;
    }

    public void StartFadeOut()
    {
        if (_isPlaying) return;
        StartCoroutine(FadeOutShadow());
    }

    IEnumerator FadeOutShadow()
    {
        _isPlaying = true;
        float fullDuration = _duration;
        float startTime = Time.time;

        while (Time.time - startTime < fullDuration)
        {
            float t = (Time.time - startTime) / fullDuration;
            float smoothT = Mathf.SmoothStep(1, 0, t);
            //float alpha1 = Mathf.Lerp(1f, 0f, smoothT);

            GetComponent<SpriteRenderer>().color = new Color(_color.r, _color.g, _color.b, smoothT);

            yield return null;
        }

        _isPlaying = false;

        Destroy(gameObject);
    }
}
