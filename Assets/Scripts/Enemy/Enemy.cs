using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// made by KimDaehui
public class Enemy : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackDelay = 1f;
    [SerializeField] public AudioClip _readySFX;
    [SerializeField] public AudioClip _AttackSFX;
    [SerializeField] AudioClip DeadSound;
    #endregion

    #region PrivateVariables
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    WaveManager _waveManager;
    #endregion

    #region ProtectedVariables
    protected Transform _playerTransform;
    #endregion

    #region PublicVariables
    public GameObject _weapon;

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
        SetStartState();
    }

    private void SetStartState()
    {
        EnemyPatrol enemyPatrol;

        if(TryGetComponent<EnemyPatrol>(out enemyPatrol))
        {
            _currentState = new PatrolState();
        }
        else
        {
            _currentState = new ChaseState();
        }
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

    public void Dead(Vector3 aimPos)
    {
        BloodEffect bloodEffect = FindObjectsOfType<BloodEffect>().Where(x=>x.IsEnemy).First();
        _audioSource.PlayOneShot(DeadSound);
        if (bloodEffect != null)
        {
            float angle = Mathf.Atan2(aimPos.y, aimPos.x) * Mathf.Rad2Deg - 90f;
            bloodEffect.InstantiateBloodEffect(transform.position, angle);
        }
        Direction.Instance.Show_Flash_Effect();
        CameraShake.Instance.shakeCamera(5f, .1f);
        Destroy(gameObject);
    }

    public float GetAttackDelay()
    {
        return _attackDelay;
    }


    public Vector2 GetdirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
	}

    public void CalcSound_Direction_Distance()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _audioSource.panStereo = (player.transform.position.x - transform.position.x) / -10;
        float final_Sound = (5f / Vector2.Distance(player.transform.position, transform.position));
        _audioSource.volume = final_Sound >= 1 ? 1 : final_Sound;
    }
    #endregion
}
