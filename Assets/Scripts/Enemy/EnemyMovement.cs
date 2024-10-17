using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using EnemyState;

// made by KimDaehui
public class EnemyMovement : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] GameObject _enemyShadow;
    [Header("")]


    #endregion

    #region PrivateVariables
    NavMeshAgent _navMeshAgent;
    EnemyAttack _enemyAttack;
    SpriteRenderer _renderer;
    #endregion

    #region ProtectedVariables
    protected Transform _playerTransform;
    [SerializeField] protected float _moveSpeed = 5f;
    #endregion

    #region PublicVariables
    [SerializeField] public bool IsActive = true;
    [HideInInspector] public StateMachine CurrentState;
    [HideInInspector] public AudioSource AudioSource;
    #endregion

    #region ProtectedMethods
    protected virtual void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        AudioSource = GetComponent<AudioSource>();
        _enemyAttack = GetComponent<EnemyAttack>();
        _renderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = _moveSpeed;
        }
        SetStartState();
    }

    void SetStartState()
    {
        CurrentState = new SleepState();
        CurrentState.EnterState(gameObject);
    }
    void Update()
    {
        transform.rotation = Quaternion.identity;

        if (CurrentState != null)
        {
            CurrentState.UpdateState(gameObject);
        }
    }
    void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedUpdateState(gameObject);
        }
    }
    #endregion

    #region PublicMethods

    public virtual void Chase()
    {
        if (_enemyAttack != null && !_enemyAttack.IsInAttackRange())
        {
            _navMeshAgent.SetDestination(_playerTransform.position);
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

    #endregion
}
