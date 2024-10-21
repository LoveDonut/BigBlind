using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmpColorModifier : MonoBehaviour
{
    [SerializeField] List<Color> colors;
    [SerializeField] float _bpmMultiplier;
    [SerializeField] bool _isUsingWaveManagerBPM = true;
    WaveManager waveManager;
    Color color => waveManager.WaveColor;
    float BPM => SoundManager.Instance.BPM *_bpmMultiplier;
    float changeInterval => 60/BPM;

    void Awake()
    {
        waveManager = GetComponent<WaveManager>();  
    }

    void Start()
    {
        if (_isUsingWaveManagerBPM)
        {
            _bpmMultiplier = waveManager._bpmMultiplier;
        }
        StartCoroutine(ColorModifyRoutine());
    }

    void Update()
    {

    }

    IEnumerator ColorModifyRoutine()
    {
        int colorIndex = 0; 

        while (true) 
        {
            waveManager.WaveColor = colors[colorIndex];
            colorIndex = (colorIndex + 1) % colors.Count;
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
