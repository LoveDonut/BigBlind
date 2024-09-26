using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region PrivateVariables
    Transform _playerTransform;
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    WaveManager _waveManager;
    #endregion

    #region ProtectedVariables
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _attackDelay = 1f;
    #endregion

    #region PublicVariables
    public StateMachine _currentState;
    #endregion

    #region PrivateVariables
    void OnDrawGizmos()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (_playerTransform.position - transform.position).normalized * _attackRange);
    }
    #endregion

    #region ProtectedMethods
    protected virtual void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waveManager = GetComponent<WaveManager>();
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

    public bool IsInAttackRange()
    {
        // check if the player is closed to enemy enough to attack
        if(_attackRange >= Vector2.Distance(transform.position, _playerTransform.position))
        {
            // check if there is no wall between enemy and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, _attackRange, LayerMask.GetMask("Wall"));
            
            if(hit.collider == null)
            {
                Debug.Log("Find Player!");
                return true;
            }
            else
            {
                Debug.Log($"There is a wall {hit.collider.gameObject.name}");
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
