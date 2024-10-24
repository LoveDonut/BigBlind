using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    #region PrivateVariables
    [Header("References")]
    [SerializeField] GameObject _explosionCircle;

    [Header("")]

    [SerializeField] float _detectRadius = 2f;
    [SerializeField] float _triggerRadius = 1f;
    [SerializeField] float _audioIncreaseOffset = 1f;
    [SerializeField] float _bpmIncreaseOffset = 1f;

    WaveManager _waveManager;
    AudioSource _audioSource;
    Transform _playerTransform;

    bool _hasBeatOn;
//    float _startAudioVolume;
    float _startBpm;
    #endregion

    #region PrivateMethods
    void Awake()
    {
        _waveManager = GetComponent<WaveManager>();
        _audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;

        //if (_audioSource != null)
        //{
        //    _startAudioVolume = _audioSource.volume;
        //}

        if(_waveManager != null)
        {
            _startBpm = _waveManager.BPM;
        }
    }

    void Update()
    {
        ChangeAudioAndBpmByDistance();
    }

    void ChangeAudioAndBpmByDistance()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceFromPlayer > _detectRadius)
        {
            if (_audioSource != null)
            {
                _audioSource.volume = 0f;
            }

            if (_hasBeatOn)
            {
                TurnOffWave();
            }


        }
        else if (distanceFromPlayer > _triggerRadius)
        {
            float distanceDiff = Mathf.Clamp(_detectRadius - distanceFromPlayer, float.MinValue, _detectRadius);

            // change audio
            _audioSource.volume = distanceDiff * _audioIncreaseOffset;

            if (!_hasBeatOn)
            {
                TurnOnWave();
            }
            else // change BPM
            {
                _waveManager.BPM = _startBpm * distanceDiff * _bpmIncreaseOffset;
            }
        }
    }

    void TurnOffWave()
    {
        if (_waveManager != null)
        {
            _waveManager.StopWave();
        }
        _hasBeatOn = false;
    }

    void TurnOnWave()
    {
        if (_waveManager != null)
        {
            _waveManager.EnqueueWaveForPlayingByBeat();
            _waveManager.BPM = _startBpm;
        }

        _hasBeatOn = true;
    }

    void Explode()
    {
        Instantiate(_explosionCircle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _triggerRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }
    #endregion
}
