using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyState;

public abstract class EnemyAttack : MonoBehaviour, ILightable
{
    #region References
    [Header("References")]
    [SerializeField] public AudioClip _readySFX;
    [SerializeField] public AudioClip _attackSFX;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackDelay = 1f;
    [SerializeField] protected int _readyBeatCount = 2;

    #endregion

    #region ProtectedVariables
    protected Transform _playerTransform;
    #endregion

    #region PublicVariables
    public float _bpmMultiplier = 1.5f; // more bigger, more faster
    public GameObject Weapon;
    public int CurrentReadyBeatCount { get; private set; }
    public bool IsLighted { get; set; }
    #endregion

    #region PrivateVariables
    #endregion

    #region PrivateMethods

    #endregion

    #region ProtectedMethods
    protected virtual void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
    }
    protected void InitWave()
    {
        WaveManager waveManager;

        if (TryGetComponent<WaveManager>(out waveManager))
        {
            waveManager.EnqueueWaveForPlayingByBeat();
        }
    }
    #endregion

    #region PublicMethods
    // default : close-range
    public virtual void ResetReadyBeatCount() => CurrentReadyBeatCount = IsLighted ? 0 : _readyBeatCount;
    public virtual void StartAttack()
    {
        ResetReadyBeatCount();
        AttackState attackState = new AttackState();
        EnemyMovement enemyMovement;

        if (TryGetComponent<EnemyMovement>(out enemyMovement))
        {
            enemyMovement.CurrentState.SwitchState(gameObject, ref enemyMovement.CurrentState, attackState);
        }
    }
    public virtual void EndSleep()
    {
        InitWave();
    }

    public virtual void InitChase()
    {
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();


        ResetReadyBeatCount();

        // start move if no player in attack range
        if (!IsInAttackRange())
        {
            if (enemyMovement != null)
            {
                enemyMovement.StartMove();
            }
        }
    }
    public virtual void EndChase()
    {

    }

    public virtual void InitAttack()
    {

    }

    public virtual void UpdateAttack()
    {

    }
    public virtual void FixedUpdateAttack()
    {

    }
    public virtual void EndAttack()
    {

    }

    public virtual bool IsInAttackRange()
    {
        if(_playerTransform == null) return false;
        // check if the player is closed to enemy enough to attack
        if (_attackRange >= Vector2.Distance(transform.position, _playerTransform.position))
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
    public float GetAttackDelay()
    {
        return _attackDelay;
    }
    public Vector2 GetDirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }

    public bool IsReadyToAttack() => CurrentReadyBeatCount-- <= 0;
    #endregion

}
