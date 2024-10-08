using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN, original script is EnemyShadow.cs
public class SeeByWave : MonoBehaviour
{
    [SerializeField] float _duration = 1f;
    bool _isPlaying;
    Color _color;

    private void Start()
    {
        _color = GetComponent<SpriteRenderer>().color;
    }

    public void StartFadeOut()
    {
        if (_isPlaying) return;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        _isPlaying = true;
        float fullDuration = _duration;
        float startTime = Time.time;

        while(Time.time - startTime < fullDuration)
        {
            float t = (Time.time - startTime) / fullDuration;
            float smoothT = Mathf.SmoothStep(1, 0, t);
            GetComponent<SpriteRenderer>().color = new Color(_color.r, _color.g, _color.b, smoothT);
            yield return null;
        }
        _isPlaying = false;
    }
}
