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
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] GameObject _enemyShadow;


    [Header("SoundPlay")]
    [SerializeField] float _stereoPanAmount = -10;
    [SerializeField] float _finalSoundNumerator = 5;


    #endregion

    #region PrivateVariables
    NavMeshAgent _navMeshAgent;
    EnemyAttack _enemyAttack;
    SpriteRenderer _renderer;
    #endregion

    #region ProtectedVariables
    protected Transform _playerTransform;
    #endregion

    #region PublicVariables
    [HideInInspector] public StateMachine CurrentState;
    [HideInInspector] public AudioSource AudioSource;
    #endregion

    #region PrivateVariables
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
        _navMeshAgent.speed = _moveSpeed;
        SetStartState();
    }

    private void SetStartState()
    {
        EnemyPatrol enemyPatrol;

        if(TryGetComponent<EnemyPatrol>(out enemyPatrol))
        {
            CurrentState = new PatrolState();
        }
        else
        {
            CurrentState = new ChaseState();
        }
        CurrentState.EnterState(gameObject);
    }

    protected virtual void Update()
    {
        transform.rotation = Quaternion.identity;
        CurrentState.UpdateState(gameObject);
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

    public void SpawnSprite()
    {
        var shadow = Instantiate(_enemyShadow, transform.position, Quaternion.identity);
        shadow.GetComponent<SpriteRenderer>().sprite = _renderer.sprite;
        shadow.GetComponent<EnemyShadow>().StartFadeOut();
    }

    public void CalcSound_Direction_Distance()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        AudioSource.panStereo = (player.transform.position.x - transform.position.x) / _stereoPanAmount;
        float final_Sound = (_finalSoundNumerator / Vector2.Distance(player.transform.position, transform.position));
        AudioSource.volume = final_Sound >= 1 ? 1 : final_Sound;
    }
    #endregion
}
