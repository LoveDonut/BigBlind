using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeHaste : MonoBehaviour
{
    [SerializeField] float _mult = 1.5f;
    [SerializeField] float _duration = 10f;
    [SerializeField] Color _hasteColor = Color.red;

    [SerializeField] AudioClip _beatSwitchSFX;
    [SerializeField] AudioClip _defaultBGM;
    [SerializeField] AudioClip _feverBGM;

    private GameObject _player;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Color _normalColor;

    bool _isEnd = false;

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
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
            PlayBeatSwitch();
        }
    }

    void PlayBeatSwitch()
    {

        SoundManager.Instance.PlaySound(_beatSwitchSFX, Vector2.zero);
        Invoke(_isEnd ? nameof(ResetToNormal) : nameof(NewBeatStart) , _beatSwitchSFX.length);
        if (_isEnd) Direction.Instance.HideAudioSpectrum();
        _isEnd = !_isEnd;
    }

    void NewBeatStart()
    {
        var bgm = SoundManager.Instance.BGMaudio;

        if (_player.GetComponent<PlayerShoot>().IsHaste) return;
        StartCoroutine(_player.GetComponent<PlayerShoot>().Haste(_mult, _duration));
        _player.GetComponent<WaveManager>().WaveColor = _hasteColor;

        // active audio spectrum
        Direction.Instance.ShowAudioSpectrum();

        // Remove these lines if music pitch isn't changed
        _player.GetComponent<WaveManager>().BPM *= _mult;
        bgm.volume *= _mult;
        bgm.clip = _feverBGM;
        bgm.loop = false;
        bgm.Play();

        Invoke(nameof(PlayBeatSwitch), _duration);
        Invoke("DelayedDestroy", _duration * 2);
    }

    void ResetToNormal()
    {
        var bgm = SoundManager.Instance.BGMaudio;

        _player.GetComponent<WaveManager>().WaveColor = _normalColor;

        // Remove these lines if music pitch isn't changed
        bgm.volume /= _mult;
        _player.GetComponent<WaveManager>().BPM /= _mult;
        _player.GetComponent<Animator>().speed /= _mult;
        bgm.clip = _defaultBGM;
        bgm.loop = true;
        bgm.Play();
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
