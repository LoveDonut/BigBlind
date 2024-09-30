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

    GameObject _wave;
    Enemy _enemy;
    Color _colorBefore;

    [Header("BGM")]
    [SerializeField] AudioClip _90BPM;

    [SerializeField] bool isPlayer;

    private GameObject _player;

    private void Start()
    {
        if (!isPlayer)
        {
            InvokeRepeating("Spawn_Wave", 0, 60 / BPM);
            _player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && isPlayer) Spawn_Wave();
    }

    public void Spawn_Wave()
    {
        _wave = Instantiate(_waveObject, transform.position, Quaternion.identity);

        float dist = isPlayer ? 1 : 3 / Vector2.Distance(transform.position, _player.transform.position);
        dist = dist >= 1 ? 1 : dist;

        WaveColor = new Color(WaveColor.r, WaveColor.g, WaveColor.b, dist);
        _wave.GetComponent<SoundRayWave>().WaveColor = WaveColor;
        _wave.GetComponent<SoundRayWave>().InitWave();
        _wave.GetComponent<SoundRayWave>().Destroy_Time = DestroyTime;
        if (TryGetComponent<Enemy>(out _enemy))
        {
            Color colorToChange;
            if (_enemy._currentState.GetType() == typeof(ReadyState) && _colorBefore != WaveReadyColor)
            {
                colorToChange = WaveReadyColor;
            }
            else
            {
                colorToChange = WaveColor;
            }
            if (_colorBefore == WaveReadyColor)
            {
                colorToChange = WaveAttackColor;
                _enemy.StartAttack();
            }
            _wave.GetComponent<SoundRayWave>().WaveColor = colorToChange;
        }
        _colorBefore = _wave.GetComponent<SoundRayWave>().WaveColor;
        Destroy(_wave, DestroyTime);
    }
}
