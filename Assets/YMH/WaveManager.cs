using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] GameObject Wave_Object;
    public float BPM = 90;
    public float Destroy_Time = .5f;
    [SerializeField] Color wave_Color;
    [SerializeField] Color wave_ReadyColor;

    GameObject Wave;
    Enemy _enemy;
    Color _colorBefore;

    [Header("BGM")]
    [SerializeField] AudioClip BPM_90;

    [SerializeField] bool isPlayer;

    private void Start()
    {
        if (!isPlayer) InvokeRepeating("Spawn_Wave", 0, 60 / BPM);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && isPlayer) Spawn_Wave();
    }

    void Spawn_Wave()
    {
        Wave = Instantiate(Wave_Object, transform.position, Quaternion.identity);
        Wave.GetComponent<SoundWave>().waveManager = this;

        if (TryGetComponent<Enemy>(out _enemy))
        {
            Color colorToChange;
            if (_enemy._currentState.GetType() == typeof(ReadyState) && _colorBefore == wave_Color)
            {
                colorToChange = wave_ReadyColor;
            }
            else
            {
                colorToChange = wave_Color;
            }
            Wave.GetComponent<SoundWave>().WaveColor = colorToChange;
            if (_colorBefore == wave_ReadyColor)
            {
                _enemy.StartAttack();
            }
        }
        _colorBefore = Wave.GetComponent<SoundWave>().WaveColor;
        Destroy(Wave, Destroy_Time);
    }
}
