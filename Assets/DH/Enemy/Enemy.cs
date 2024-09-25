using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _moveSpeed = 5f;

    Transform _playerTransform;
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    WaveManager _waveManager;
    #endregion

    #region PublicVariables
    #endregion

    #region PrivateMethods
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
    }

    protected virtual void Update()
    {
        transform.rotation = Quaternion.identity;
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

    public virtual void StartReadyForAttack()
    {
        _waveManager.OnAttackReady += _waveManager.ChangeColorToReadyColor;
    }

    // default : close-range
    public virtual void StartAttack()
    {
        // attack only once
        _waveManager.OnAttack -= _waveManager.OnAttack;
        // attack player
        _animator.SetTrigger("StartAttack");
    }

    public bool IsInAttackRange()
    {
        return _attackRange >= Vector2.Distance(transform.position, _playerTransform.position);
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
    #endregion
}
