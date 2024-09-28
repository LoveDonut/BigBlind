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
    float _bleedRate =>1f/(1f +  (_maxHealth - _currentHealth)*2);
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
        BloodEffect bloodEffect = FindObjectsOfType<BloodEffect>().Where(x => !x.IsEnemy).First();
        while (true)
        {
                if (bloodEffect != null)
                {
                    float randomX = transform.position.x + UnityEngine.Random.Range(-.5f, .5f);
                    float randomY = transform.position.y + UnityEngine.Random.Range(-.5f, .5f);
                    float randomScale = UnityEngine.Random.Range(0.1f, .5f);
                    float randomRotation = UnityEngine.Random.Range(0, 360);
                    Vector2 randomPos = new Vector2(randomX, randomY);
                    bloodEffect.InstantiateBloodEffect(randomPos, randomRotation, randomScale);
                }
            float randomRate = UnityEngine.Random.Range(0.5f, 1f);
            yield return new WaitForSeconds(_bleedRate*randomRate);
        }    
    }


}
