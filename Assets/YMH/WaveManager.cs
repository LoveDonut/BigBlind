using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] GameObject Wave_Object;
    public float BPM = 90;
    public float Destroy_Time = .5f;
    [SerializeField] Color wave_Color;


    [Header("BGM")]
    [SerializeField] AudioClip BPM_90;

    private void Start()
    {
        //if(tag == "Player") GetComponent<AudioSource>().Play();
        InvokeRepeating("Spawn_Wave", 0, 60 / BPM);
    }

    void Spawn_Wave()
    {
        var wave = Instantiate(Wave_Object, transform.position, Quaternion.identity);
        wave.GetComponent<SoundWave>().waveManager = this;
        wave.GetComponent<SoundWave>().WaveColor = wave_Color;
        Destroy(wave, Destroy_Time);
    }
}