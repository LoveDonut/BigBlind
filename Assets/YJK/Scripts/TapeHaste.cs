using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeHaste : MonoBehaviour
{
    [SerializeField] float _mult = 1.5f;
    [SerializeField] float _duration = 10f;
    [SerializeField] Color _hasteColor = Color.red;
    private UnityEngine.GameObject _player;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Color _normalColor;

    private void Start()
    {
        _player = UnityEngine.GameObject.Find("Player");
        _normalColor = _player.GetComponent<WaveManager>().WaveColor;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_player.GetComponent<PlayerShoot>().IsHaste) return;
            StartCoroutine(_player.GetComponent<PlayerShoot>().Haste(_mult, _duration));
            _player.GetComponent<WaveManager>().WaveColor = _hasteColor;

            // Remove these lines if music pitch isn't changed
            _player.GetComponent<WaveManager>().BPM *= _mult;
            _player.GetComponent<AudioSource>().pitch *= _mult;
            _player.GetComponent<Animator>().speed *= _mult;

            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            Invoke("ResetToNormal", _duration);
            Invoke("DelayedDestroy", _duration * 2);
        }
    }

    void ResetToNormal()
    {
        _player.GetComponent<WaveManager>().WaveColor = _normalColor;

        // Remove these lines if music pitch isn't changed
        _player.GetComponent<WaveManager>().BPM /= _mult;
        _player.GetComponent<AudioSource>().pitch /= _mult;
        _player.GetComponent<Animator>().speed /= _mult;
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
