using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] GameObject Wave_Object;
    public float BPM = 90;
    public float Destroy_Time = .5f;
    public float Cannon_Destroy_Time = .5f;
    public Color wave_Color;
    [SerializeField] Color wave_ReadyColor;
    [SerializeField] Color wave_AttackColor;

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
        
        Wave.GetComponent<SoundRayWave>().WaveColor = wave_Color;
        Wave.GetComponent<SoundRayWave>().InitWave();
        Wave.GetComponent<SoundRayWave>().Destroy_Time = Destroy_Time;
        if (TryGetComponent<Enemy>(out _enemy))
        {
            Color colorToChange;
            if (_enemy._currentState.GetType() == typeof(ReadyState) && _colorBefore != wave_ReadyColor)
            {
                colorToChange = wave_ReadyColor;
            }
            else
            {
                colorToChange = wave_Color;
            }
            if (_colorBefore == wave_ReadyColor)
            {
                colorToChange = wave_AttackColor;
                _enemy.StartAttack();
            }
            Wave.GetComponent<SoundRayWave>().WaveColor = colorToChange;
        }
        _colorBefore = Wave.GetComponent<SoundRayWave>().WaveColor;
        Destroy(Wave, Destroy_Time);
    }
}
