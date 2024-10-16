using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by JHC
public class PlayerFlashBang : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] FlashBomb _flashBomb;
    [SerializeField] GameObject _handCannonWave;

    [Header("Variable")]
    [SerializeField] int _flashBombCount;
    [SerializeField] float _coolTime;
    [SerializeField] Color _waveColor;
    [SerializeField] float _waveDestoryTime;
    bool _isPlaying;
    void OnFlashBang()
    {
        if (_flashBombCount > 0 && !_isPlaying)
        {
            _flashBombCount--;
            _flashBomb.Flash();
            SpawnWave();
            StartCoroutine(coolTime());
        }
    }

    void SpawnWave()
    {
        var wave = Instantiate(_handCannonWave, transform.position, Quaternion.identity);
        wave.GetComponent<SoundRayWave>().isCannonWave = true;
        wave.GetComponent<SoundRayWave>().WaveColor = _waveColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = _waveDestoryTime;
    }

    private IEnumerator coolTime()
    {
        _isPlaying = true;
        yield return new WaitForSeconds(_coolTime);
        _isPlaying = false;
        
    }


}
