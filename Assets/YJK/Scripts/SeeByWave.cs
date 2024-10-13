using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN, original script is EnemyShadow.cs
public class SeeByWave : MonoBehaviour
{
    [SerializeField] float _duration = 1f;
    Color _color;
    float _timeLeft;

    private void Start()
    {
        _color = GetComponent<SpriteRenderer>().color;
        _timeLeft = 0f;
    }
    
    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(_color.r, _color.g, _color.b, _timeLeft / _duration);
    }

    public void StartFadeOut()
    {
        _timeLeft = _duration;
    }
}
