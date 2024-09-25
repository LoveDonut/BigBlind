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

    public Action OnAttack;
    public Action OnAttackReady;
    GameObject Wave;

    [Header("BGM")]
    [SerializeField] AudioClip BPM_90;

    [SerializeField] bool isPlayer;

    private void Start()
    {
        if(!isPlayer) InvokeRepeating("Spawn_Wave", 0, 60 / BPM);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && isPlayer) Spawn_Wave();
    }

    void Spawn_Wave()
    {
        Wave = Instantiate(Wave_Object, transform.position, Quaternion.identity);
        Wave.GetComponent<SoundWave>().waveManager = this;
        Wave.GetComponent<SoundWave>().WaveColor = wave_Color;
        OnAttack?.Invoke();
        OnAttackReady?.Invoke();
        Destroy(Wave, Destroy_Time);
    }

    public void ChangeColorToReadyColor()
    {
        Color waveColor = Wave.GetComponent<SoundWave>().WaveColor;
        //if(waveColor == wave_ReadyColor)
        //{            
        //    Enemy enemyComponent;
        //    if (TryGetComponent<Enemy>(out enemyComponent))
        //    {
        //        enemyComponent.StartAttack();
        //    }
        //}
        Wave.GetComponent<SoundWave>().WaveColor = wave_ReadyColor;

        // ready only once
        OnAttackReady -= OnAttackReady;

        Enemy enemyComponent;
        if (TryGetComponent(out enemyComponent))
        {
            // attack after ready
            OnAttack += enemyComponent.StartAttack;
        }
    }
}
