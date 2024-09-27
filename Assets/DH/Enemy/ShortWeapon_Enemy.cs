using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortWeapon_Enemy : MonoBehaviour
{
    Animator _animator;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartAttack()
    {
        if (TryGetComponent<Animator>(out _animator))
        {
            _animator.SetTrigger("ShortAttack");
        }
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }
}
