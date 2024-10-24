using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] GameObject _waveObject;

    // editted by Daehui
    [HideInInspector] public bool WillBeOff;

    [HideInInspector]
    public float BPM = 90;

    public float _bpmMultiplier = 1f;
    public float DestroyTime = .5f;
    public float CannonDestroyTime = .5f;
    public Color WaveColor;
    [SerializeField] Color WaveReadyColor;
    [SerializeField] Color WaveAttackColor;

    [SerializeField] float _blockAlphaAmount = 2;
    [SerializeField] float _distanceFadeNumerator = 3;

    GameObject _wave;
    EnemyAttack _enemyAttack;
    EnemyMovement _enemyMovement;

    [SerializeField] bool isPlayer;
    [SerializeField] bool _repeatWave = true;

    private GameObject _player;

    float _dist;

    bool _isReadyAttack;
    [SerializeField] GameObject _waveEffect;

    // editted by Daehui
    Coroutine _waveCoroutine;
    Color _startColor;

    void Start()
    {
        BPM = SoundManager.Instance.BPM * _bpmMultiplier;
        if (!CompareTag("Enemy"))
        {
            StartWaveByBeat();
        }

        // editted by Daehui
        _startColor = WaveColor;
    }

    public void EnqueueWaveForPlayingByBeat()
    {
        TimeManager.Instance._waveManagers.Enqueue(this);
    }

    public void StartWaveByBeat()
    {

        if (!isPlayer)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        if (_repeatWave)
        {
            SpawnWave(true);
        }
    }

    // editted by Daehui
    public void StopWave()
    {
        if(_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            WillBeOff = false;
        }
        else
        {
            WillBeOff = true;
        }
    }

    public void SpawnWave(bool isRepeat = false)
    {
        // editted by Daehui
        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
        }
        _waveCoroutine = StartCoroutine(SpawnWaveCoroutine(isRepeat));
    }

    // editted by Daehui
    public void RestartWave(Collider2D hit, float EMPDuration)
    {
        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
        }
        _waveCoroutine = StartCoroutine(RestartWaveCoroutine(hit, EMPDuration));
    }

    IEnumerator RestartWaveCoroutine(Collider2D hit, float EMPDuration)
    {
        yield return new WaitForSeconds(EMPDuration);
        TimeManager.Instance._waveManagers.Enqueue(this);
    }

    IEnumerator SpawnWaveCoroutine(bool isRepeat = false)
    {
        do
        {
            if (isPlayer && CompareTag("Player"))
            {
                if (GetComponent<PlayerHealth>().CurrentHp <= 0) break;
                var wave = Instantiate(_waveEffect, transform.position, Quaternion.identity);
                wave.GetComponent<LineRenderer>().material.SetColor("_BaseColor", WaveColor);

            }

            _wave = Instantiate(_waveObject, transform.position, Quaternion.identity);

            _dist = isPlayer ? 1 : _distanceFadeNumerator / Vector2.Distance(transform.position, _player.transform.position);
            _dist = _dist >= 1 ? 1 : _dist;

            WaveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, (isPlayer || (!isPlayer && _isReadyAttack)) ? 1 : (IsBlockedByWalls() ? _dist / _blockAlphaAmount : _dist));
            _wave.GetComponent<SoundRayWave>().WaveColor = WaveColor;
            _wave.GetComponent<SoundRayWave>().InitWave();
            _wave.GetComponent<SoundRayWave>().Destroy_Time = DestroyTime;
            ChangeWaveColorAccordingToState();
            if (CompareTag("Enemy"))
            {
                _wave.GetComponent<LineRenderer>().material.SetColor("_BaseColor", WaveColor);
                GetComponent<EnemyWaveOnBlood>().WaveOnBlood();
            }
            Destroy(_wave, DestroyTime);
            yield return new WaitForSeconds(60 / BPM);
        } while (isRepeat);
    }

    private void ChangeWaveColorAccordingToState()
    {
        if (TryGetComponent<EnemyAttack>(out _enemyAttack) && TryGetComponent<EnemyMovement>(out _enemyMovement))
        {
            Color colorToChange;
            if (_enemyMovement.CurrentState != null && _enemyMovement.CurrentState.GetType() == typeof(EnemyState.ReadyState))
            {
                colorToChange = WaveReadyColor;
                _isReadyAttack = true;
                if (_enemyAttack.IsReadyToAttack())
                {
                    _isReadyAttack = false;
                    colorToChange = WaveAttackColor;
                    _enemyAttack.StartAttack();
                }
            }
            else
            {
                colorToChange = _startColor;
            }

            WaveColor = colorToChange;
        }
    }

    bool IsBlockedByWalls()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, Vector2.Distance(transform.position, _player.transform.position), 1 << 8);

        return hit.collider != null;
    }

}
