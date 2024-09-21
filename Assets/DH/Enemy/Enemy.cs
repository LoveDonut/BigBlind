using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float _attackRange = 1f;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _readyDelay = 1f;

    Transform _playerTransform;
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    #endregion

    #region PublicVariables
    #endregion

    #region PrivateMethods
    void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    void Start()
    {
        _navMeshAgent.speed = _moveSpeed;
    }
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    IEnumerator ReadyDelay()
    {
        yield return new WaitForSeconds(_readyDelay);
        _animator.SetTrigger("StartAttack");
    }
    #endregion

    #region PublicMethods

    public virtual void Chase()
    {
        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    public virtual void ReadyForAttack()
    {
        Debug.Log("Ready Attack");
        StartCoroutine(ReadyDelay());
    }

    // default : close-range
    public virtual void Attack()
    {
        // attack player
        Debug.Log("Attack");
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
