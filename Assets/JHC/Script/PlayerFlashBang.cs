using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by JHC
public class PlayerFlashBang : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] FlashBomb _flashBomb;

    [Header("Variable")]
    [SerializeField] int _flashBombCount;
    [SerializeField] float _coolTime;
    bool _isPlaying;
    void OnFlashBang()
    {
        if (_flashBombCount > 0 && !_isPlaying)
        {
            _flashBombCount--;
            _flashBomb.Flash();
            StartCoroutine(coolTime());
        }
    }
    private IEnumerator coolTime()
    {
        _isPlaying = true;
        yield return new WaitForSeconds(_coolTime);
        _isPlaying = false;
    }
}
