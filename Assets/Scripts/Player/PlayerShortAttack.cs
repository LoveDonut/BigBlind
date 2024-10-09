using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using UnityEngine.Windows;

// made by KimDaehui
public class PlayerShortAttack : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Transform _hitTransform;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] float _hitRadius = 0.8f;
    [SerializeField] float _tackleAcceleration = 10f;
    [SerializeField] float _tackleDeceleration = 10f;
    [SerializeField] float _maxTackleSpeed = 10f;
    [SerializeField] float _tackleAccelerationTime = 0.5f;
    [SerializeField] float _tackleDecelerationTime = 0.25f;
    [Tooltip("Must bigger than accelerationTime + decelerationTime")]
    [SerializeField] float _shortAttackCoolTime = 1f;

    Coroutine _attackCoroutine;
    Animator _animator;
    PlayerMovement _playerMovement;
    Rigidbody2D _rigidbody;
    #endregion

    #region PublicVariables
    [HideInInspector] public Vector2 TackleVelocity;
    [HideInInspector] public float TackleElapsedTime;
    [HideInInspector] public bool CanAttack;
    #endregion

    #region PrivateMethods

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        CanAttack = true;
        TackleElapsedTime = _tackleAccelerationTime + _tackleDecelerationTime;

        if(_shortAttackCoolTime < TackleElapsedTime)
        {
            _shortAttackCoolTime = TackleElapsedTime;
        }
    }

    void OnShortAttack()
    {
        if (!_playerMovement.IsMovable) return;

        if (_animator != null && _playerMovement != null && CanAttack)
        {
            _playerMovement.CurrentState.SwitchState(gameObject, ref _playerMovement.CurrentState, new ShortAttackState());

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
            _attackCoroutine = StartCoroutine(CoolDown());
        }
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_shortAttackCoolTime);
        CanAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_hitTransform.position, _hitRadius);
    }
    #endregion

    #region PublicMethods
    public void AttackCollideWithEnemy()
    {
        Collider2D hit = Physics2D.OverlapCircle(_hitTransform.position, _hitRadius, LayerMask.GetMask("Enemy", "Glass", "Box"));
        IDamage damagable;

        if (hit != null && hit.gameObject != gameObject && hit.TryGetComponent<IDamage>(out damagable))
        {
            damagable.GetDamaged((_hitTransform.position - transform.position).normalized);
        }
    }

    public void Tackle(Vector2 tackleDirection)
    {
        if (_rigidbody == null) return;

        TackleElapsedTime -= Time.deltaTime;
        // set acceleration and decceleration
        if (TackleElapsedTime > _tackleDecelerationTime)
        {
            TackleVelocity += tackleDirection * _tackleAcceleration * Time.deltaTime;
        }
        else if (TackleElapsedTime > 0)
        {
            TackleVelocity = Vector2.MoveTowards(TackleVelocity, Vector2.zero, _tackleDeceleration * Time.deltaTime);
        }
        else
        {
            _playerMovement.CurrentState.SwitchState(gameObject, ref _playerMovement.CurrentState, new IdleState());
        }

        // limit max speed
        TackleVelocity = Vector2.ClampMagnitude(TackleVelocity, _maxTackleSpeed);

        // set speed
        _rigidbody.MovePosition(_rigidbody.position + TackleVelocity * Time.fixedDeltaTime);
    }
    #endregion
}
