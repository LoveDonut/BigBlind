using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackDelay = 1f;
    [SerializeField] public AudioClip _readySFX;
    [SerializeField] public AudioClip _attackSFX;
    #endregion

    #region PrivateVariables
    #endregion

    #region ProtectedVariables
    protected Transform _playerTransform;
    #endregion

    #region PublicVariables
    public GameObject _weapon;
    #endregion

    #region PrivateVariables
    #endregion

    #region ProtectedMethods
    void Awake()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
    }
    #endregion

    #region PublicMethods
    // default : close-range
    public virtual void ChangeToAttackStateAccordingToBeat()
    {

    }
    public virtual void StartAttack()
    {
        AttackState attackState = new AttackState();
        EnemyMovement enemyMovement;

        if(TryGetComponent<EnemyMovement>(out enemyMovement))
        {
            enemyMovement._currentState.SwitchState(enemyMovement, attackState);
        }
    }
    public virtual bool IsInAttackRange()
    {
        // check if the player is closed to enemy enough to attack
        if (_attackRange >= Vector2.Distance(transform.position, _playerTransform.position))
        {
            float rayLength = _attackRange > (_playerTransform.position - transform.position).magnitude ?
                                (_playerTransform.position - transform.position).magnitude : _attackRange;
            // check if there is no wall between enemy and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, rayLength, LayerMask.GetMask("Wall"));

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
    public Vector2 GetdirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }
    #endregion

}