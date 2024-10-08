using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public class PlayerShortAttack : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Transform _hitTransform;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] float _shortAttackCoolTime = 1f;
    [SerializeField] float _hitRadius = 0.8f;

    bool _canAttack;
    Coroutine _attackCoroutine;
    Animator _animator;
    #endregion

    #region PrivateMethods

    void Start()
    {
        _canAttack = true;
        _animator = GetComponent<Animator>();
    }

    void OnShortAttack()
    {
        if (_animator != null && _canAttack)
        {
            _canAttack = false;
            _animator.SetTrigger("DoShortAttack");

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
        _canAttack = true;
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
    #endregion
}
