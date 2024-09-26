using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_LongRange : Enemy
{
    [SerializeField] GameObject _bulletPrefab;
    float _bulletRadius;
    protected override void Start()
    {
        base.Start();
        CircleCollider2D bulletCollider;

        if(_bulletPrefab.TryGetComponent<CircleCollider2D>(out bulletCollider))
        {
            _bulletRadius = bulletCollider.radius;
        }
    }

    public void Fire()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + (Vector3)DirectionToPlayer(), Quaternion.LookRotation(DirectionToPlayer().normalized));

        Destroy(bullet, 3f);
    }

    public override bool IsInAttackRange()
    {
        // check if the player is closed to enemy enough to attack
        if (_attackRange >= Vector2.Distance(transform.position, _playerTransform.position))
        {
            float rayLength = _attackRange > (_playerTransform.position - transform.position).magnitude ?
                                (_playerTransform.position - transform.position).magnitude : _attackRange;
            // check if there is no wall between enemy and player
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, _bulletRadius, _playerTransform.position - transform.position, rayLength, LayerMask.GetMask("Wall"));

            if (hit.collider == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
