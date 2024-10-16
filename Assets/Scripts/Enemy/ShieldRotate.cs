using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using EnemyState;

// made by Daehui
public class ShieldRotate : MonoBehaviour
{
    Transform _playerTransform;
    EnemyMovement _enemyMovement;
    ShielderAttack _shielderAttack;

    float rotationSpeed;

    void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        _enemyMovement = GetComponentInParent<EnemyMovement>();
        _shielderAttack = GetComponentInParent<ShielderAttack>();
    }

    void Update()
    {
        if (_enemyMovement != null)
        {
            if(_enemyMovement.CurrentState.GetType() == typeof(ChaseState) || _enemyMovement.CurrentState.GetType() == typeof(ReadyState))
            {
                Rotate();
            }
        }
    }

    public void Rotate()
    {
        if(_playerTransform == null || _shielderAttack == null) return;

        Vector2 direction = (_playerTransform.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angle = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, targetAngle, _shielderAttack.ShieldRotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
