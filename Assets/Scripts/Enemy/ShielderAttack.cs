using PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;
using UnityEngineInternal;

public class ShielderAttack : EnemyAttack
{
    #region References
    [Header("References")]
    [SerializeField] GameObject _hitTransform;
    [Header("")]
    #endregion

    #region PrivateVariables
    [SerializeField] Vector2 _hitSize;
    [SerializeField] float _rushSpeed = 20f;
    [SerializeField] float ShortAttackDistance = 3f;
    [SerializeField] float angleThreshold = 20f;

    Rigidbody2D _rigidbody;
    Vector2 _directionToPlayer;
    Vector2 _startPosition;
    #endregion

    #region PublicVariables
    public float ShieldRotationSpeed = 100f;

    [Header("Knockback")]
    public float KnockbackSpeed = 20f;
    public float KnockbackDistance = 3f;
    public float KnockbackDecceleration = 50f;
    [HideInInspector] public Vector2 KnockbackDirection;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(_hitTransform.transform.position, Quaternion.Euler(0, 0, Weapon.transform.localEulerAngles.z), Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, _hitSize);
    }
    void CollideWithShield()
    {
        Collider2D[] hits = GetHittedColliderAtBox();

        foreach (Collider2D hit in hits)
        {
//            IDamage damagable = hit.GetComponent<IDamage>() != null ? hit.GetComponent<IDamage>() : hit.GetComponentInParent<IDamage>();
            IDamage damagable = hit.GetComponent<IDamage>();

            // Attack
            if (hit.gameObject != gameObject && damagable != null)
            {
                damagable.GetDamaged((Weapon.transform.position - transform.position).normalized);
            }
        }

    }
    Collider2D[] GetHittedColliderAtBox() => Physics2D.OverlapBoxAll(_hitTransform.transform.position, _hitSize, Weapon.transform.localEulerAngles.z, LayerMask.GetMask("Enemy", "Player", "Attack"));

    public override void InitAttack()
    {
        _directionToPlayer = GetDirectionToPlayer();
        _startPosition = transform.position;
    }

    public override void UpdateAttack()
    {
    }

    public override void EndAttack()
    {
        if (_rigidbody == null) return;

        _rigidbody.velocity = Vector3.zero;
    }

    public override void FixedUpdateAttack()
    {
        if (Vector2.Distance(_startPosition, transform.position) < ShortAttackDistance)
        {
            Tackle(_directionToPlayer);
            CollideWithShield();
        }
        else if(_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }

    public void Tackle(Vector2 tackleDirection)
    {
        if (_rigidbody == null) return;

        _rigidbody.velocity = _rushSpeed * tackleDirection;
    }

    public override bool IsInAttackRange()
    {
        if (_playerTransform == null) return false;

        float angleToPlayer = Vector2.Angle(Weapon.transform.up, GetDirectionToPlayer());

        // check if the player is closed to enemy enough to attack
        if (_attackRange >= Vector2.Distance(transform.position, _playerTransform.position) && angleToPlayer < angleThreshold)
        {
            float rayLength = _attackRange > (_playerTransform.position - transform.position).magnitude ?
                                (_playerTransform.position - transform.position).magnitude : _attackRange;
            // check if there is no wall between enemy and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, rayLength, LayerMask.GetMask("Wall", "Box"));

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
