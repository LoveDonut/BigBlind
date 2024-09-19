using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject Wave_Object;
    public float Wave_Spawn_Time = .5f;
    public float Destroy_Time = .5f;
    [SerializeField] Color wave_Color;
    private void Start()
    {
        InvokeRepeating("Spawn_Wave", 0, Wave_Spawn_Time);
    }

    void Spawn_Wave()
    {
        var wave = Instantiate(Wave_Object, transform.position, Quaternion.identity);
        wave.GetComponent<SoundWave>().waveManager = this;
        wave.GetComponent<SoundWave>().WaveColor = wave_Color;
        Destroy(wave, Destroy_Time);
    }
}
