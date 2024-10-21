using PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;
using UnityEngineInternal;

// made by Daehui
public class ShielderAttack : EnemyAttack
{
    #region References
    [Header("References")]
    public GameObject _hitTransform;
    [Header("")]
    #endregion

    #region PrivateVariables
    [SerializeField] Vector2 _hitSize;
    [SerializeField] float _rushSpeed = 20f;
    [SerializeField] float _shortAttackDistance = 3f;
    [SerializeField] float _angleThreshold = 20f;
    [SerializeField] float _firstAngleThresholdWhenTrick = 40f;

    Rigidbody2D _rigidbody;
    Vector2 _directionToPlayer;
    Vector2 _startPosition;
    #endregion

    #region PublicVariables
    public float ShieldRotationSpeed = 100f;
    public bool IsTrick;

    [Header("Knockback")]
    public float KnockbackSpeed = 20f;
    public float KnockbackDistance = 3f;
    public float KnockbackDecceleration = 50f;
    [HideInInspector] public Vector2 KnockbackDirection;
    [HideInInspector] public float defenseAngle = 90f;
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
                damagable.GetDamaged((Weapon.transform.position - transform.position).normalized, gameObject);
            }
        }

    }
    public Collider2D[] GetHittedColliderAtBox() => Physics2D.OverlapBoxAll(_hitTransform.transform.position, _hitSize, Weapon.transform.localEulerAngles.z, LayerMask.GetMask("Enemy", "Player", "Attack"));
    public override void EndSleep()
    {
        if(!IsTrick)
        {
            InitWave();
        }
    }
    public override void InitChase()
    {
        base.InitChase();
        if (IsTrick)
        {
            EnemyMovement enemyMovement;
            if(TryGetComponent<EnemyMovement>(out enemyMovement))
            {
                enemyMovement.StopMove();
            }
        }
    }
    public override void EndChase()
    {
        if (IsTrick)
        {
            InitWave();
        }
    }
    public override void InitAttack()
    {
        _directionToPlayer = IsTrick ? Weapon.transform.up : GetDirectionToPlayer();

        if (IsTrick)
        {
            EnemyMovement enemyMovement;
            if (TryGetComponent<EnemyMovement>(out enemyMovement))
            {
                enemyMovement.StartMove();
            }
        }

        _startPosition = transform.position;
    }

    public override void UpdateAttack()
    {
    }

    public override void EndAttack()
    {
        if (_rigidbody == null) return;

        // if first attack ends, become original shielder
        IsTrick = false;

        _rigidbody.velocity = Vector3.zero;
    }

    public override void FixedUpdateAttack()
    {
        if (Vector2.Distance(_startPosition, transform.position) < _shortAttackDistance)
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
        if (_attackRange >= Vector2.Distance(transform.position, _playerTransform.position) 
            && angleToPlayer < (IsTrick? _firstAngleThresholdWhenTrick : _angleThreshold))
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
