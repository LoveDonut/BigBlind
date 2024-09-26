using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region PrivateVariables
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    WaveManager _waveManager;
    #endregion

    #region ProtectedVariables
    [Header("Move")]
    [SerializeField] protected float _moveSpeed = 5f;
    [Header("Attack")]
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackDelay = 1f;

    protected Transform _playerTransform;
    #endregion

    #region PublicVariables
    [Header("SFX")]
    [SerializeField] public AudioClip _readySFX;
    [SerializeField] public AudioClip _AttackSFX;

    [HideInInspector] public StateMachine _currentState;
    [HideInInspector] public AudioSource _audioSource;
    #endregion

    #region PrivateVariables
    #endregion

    #region ProtectedMethods
    protected virtual void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waveManager = GetComponent<WaveManager>();
        _audioSource = GetComponent<AudioSource>();
    }
    protected virtual void Start()
    {
        _navMeshAgent.speed = _moveSpeed;
        _currentState = new ChaseState();
        _currentState.EnterState(this);
    }

    protected virtual void Update()
    {
        transform.rotation = Quaternion.identity;
        _currentState.UpdateState(this);
    }

    protected Vector2 DirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }
    #endregion

    #region PublicMethods

    public virtual void Chase()
    {
        if (IsInAttackRange())
        {
            return;
        }
        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    // default : close-range
    public virtual void StartAttack()
    {
        AttackState attackState = new AttackState();
        _currentState.SwitchState(this,attackState);
    }

    public virtual bool IsInAttackRange()
    {
        // check if the player is closed to enemy enough to attack
        if(_attackRange >= Vector2.Distance(transform.position, _playerTransform.position))
        {
            float rayLength = _attackRange > (_playerTransform.position - transform.position).magnitude ?
                                (_playerTransform.position - transform.position).magnitude : _attackRange;
            // check if there is no wall between enemy and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, rayLength, LayerMask.GetMask("Wall"));
            
            if(hit.collider == null)
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

    public void StopMove()
    {
        _navMeshAgent.isStopped = true;
    }

    public void StartMove()
    {
        if (_navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
        }
    }

    public void Dead()
    {
        BloodEffect bloodEffect = FindObjectOfType<BloodEffect>();
        if (bloodEffect != null)
        {
            bloodEffect.InstantiateBloodEffect(transform);
        }
        Direction.Instance.Show_Flash_Effect();
        CameraShake.instance.shakeCamera(5f, .1f);
        Destroy(gameObject);
    }
    public float GetAttackDelay()
    {
        return _attackDelay;
    }
    #endregion
}
