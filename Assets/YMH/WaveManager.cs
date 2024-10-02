using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] GameObject _waveObject;
    public float BPM = 90;
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
    Color _colorBefore;

    [Header("BGM")]
    [SerializeField] AudioClip _90BPM;

    [SerializeField] bool isPlayer;

    private GameObject _player;

    float _dist;

    private void Start()
    {
        if (!isPlayer)
        {
            InvokeRepeating("Spawn_Wave", 0, 60 / BPM);
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        _colorBefore = WaveAttackColor;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && isPlayer) Spawn_Wave();
    }

    public void Spawn_Wave()
    {
        _wave = Instantiate(_waveObject, transform.position, Quaternion.identity);

        _dist = isPlayer ? 1 : _distanceFadeNumerator / Vector2.Distance(transform.position, _player.transform.position);
        _dist = _dist >= 1 ? 1 : _dist;

        WaveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, isPlayer ? 1 : (IsBlockedByWalls() ? _dist / _blockAlphaAmount : _dist));
        _wave.GetComponent<SoundRayWave>().WaveColor = WaveColor;
        _wave.GetComponent<SoundRayWave>().InitWave();
        _wave.GetComponent<SoundRayWave>().Destroy_Time = DestroyTime;
        ChamgeWaveColorAccordingToState();
        if(CompareTag("Enemy"))
        {
            GetComponent<EnemyWaveOnBlood>().WaveOnBlood();
        }
        Destroy(_wave, DestroyTime);
    }

    private void ChamgeWaveColorAccordingToState()
    {
        if (TryGetComponent<EnemyAttack>(out _enemyAttack) && TryGetComponent<EnemyMovement>(out _enemyMovement))
        {
            Color colorToChange;
            if (_enemyMovement._currentState.GetType() == typeof(ReadyState))
            {
                colorToChange = WaveReadyColor;
            }
            else
            {
                colorToChange = WaveColor;
            }
            if (_colorBefore == WaveReadyColor && _enemyAttack.IsReadyToAttack())
            {
                Debug.Log(("Attack!"));
                colorToChange = WaveAttackColor;
                _enemyAttack.StartAttack();
            }
            _wave.GetComponent<SoundRayWave>().WaveColor = colorToChange;
        }
        _colorBefore = _wave.GetComponent<SoundRayWave>().WaveColor;
    }

    bool IsBlockedByWalls()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, Vector2.Distance(transform.position, _player.transform.position), 1 << 8);

        return hit.collider != null;
    }

}
