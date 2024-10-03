using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortWeapon : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Transform _hitTransform;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] float _hitRadius = 0.8f;
    #endregion

    #region PrivateMethods
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_hitTransform.position, _hitRadius);
    }
    #endregion
    #region PublicMethods
    public void AttackCollideWithEnemy()
    {
        Collider2D hit = Physics2D.OverlapCircle(_hitTransform.position, _hitRadius, LayerMask.GetMask("Enemy"));
        IDamage damagable;

        if (hit != null && hit.gameObject != gameObject && hit.TryGetComponent<IDamage>(out damagable))
        {
            damagable.GetDamaged(transform.position);
        }
    }
    public void EndAttack()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
