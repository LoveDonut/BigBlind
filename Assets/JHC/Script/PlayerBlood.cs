using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//made by Chun Jin Ha
public class PlayerBlood : MonoBehaviour
{
    PlayerHealth _playerHealth;

    int _currentHealth => _playerHealth.GetCurrentHp;
    int _maxHealth => _playerHealth.GetMaxHp;
    bool _isFullHealth => _playerHealth.IsFullHealth;
    float _bleedRate =>1f/(1f +  _maxHealth - _currentHealth);
    bool _isBleeding;
    void Start()
    {
        _isBleeding = false;
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if(!_isBleeding && !_isFullHealth)
        {
            StartCoroutine(BloodEffect());
            _isBleeding = true;
        }
        else if (_isBleeding && _isFullHealth)
        {
            StopCoroutine(BloodEffect());
            _isBleeding = false;
        }
    }


    IEnumerator BloodEffect()
    {
        while (true)
        {
                BloodEffect bloodEffect = FindObjectsOfType<BloodEffect>().Where(x => !x.IsEnemy).First();
                if (bloodEffect != null)
                {
                    bloodEffect.InstantiateBloodEffect(transform.position, 0,0.3f);
                }
            
            yield return new WaitForSeconds(_bleedRate);
        }    
    }


}
