using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeHaste : MonoBehaviour
{
    [SerializeField] float _mult = 1.5f;
    [SerializeField] float _duration = 10f;
    [SerializeField] Color _hasteColor = Color.red;

    [SerializeField] AudioClip _defaultBGM;
    [SerializeField] AudioClip _feverBGM;

    private GameObject _player;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Color _normalColor;

    private void Start()
    {
        _duration = _feverBGM.length;

        _player = GameObject.Find("Player");
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

            // active audio spectrum
            Direction.Instance.ShowAudioSpectrum();

            // Remove these lines if music pitch isn't changed
            _player.GetComponent<WaveManager>().BPM *= _mult;
            _player.GetComponent<AudioSource>().volume *= _mult;
            _player.GetComponent<AudioSource>().clip = _feverBGM;
            _player.GetComponent<AudioSource>().loop = false;
            _player.GetComponent<AudioSource>().Play();

            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            Invoke("ResetToNormal", _duration);
            Invoke("DelayedDestroy", _duration * 2);
        }
    }

    void ResetToNormal()
    {

        Direction.Instance.HideAudioSpectrum();

        _player.GetComponent<WaveManager>().WaveColor = _normalColor;

        // Remove these lines if music pitch isn't changed
        _player.GetComponent<AudioSource>().volume /= _mult;
        _player.GetComponent<WaveManager>().BPM /= _mult;
        _player.GetComponent<Animator>().speed /= _mult;
        _player.GetComponent<AudioSource>().clip = _defaultBGM;
        _player.GetComponent<AudioSource>().loop = true;
        _player.GetComponent<AudioSource>().Play();
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
