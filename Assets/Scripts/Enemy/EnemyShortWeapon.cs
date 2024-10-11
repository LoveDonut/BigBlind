using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public class EnemyShortWeapon : MonoBehaviour, IParriable
{
    #region References
    [Header("References")]
    [SerializeField] Transform _hitTransform;
    [SerializeField] float _hitRadius = 0.5f;
    [SerializeField] Vector2 _weaponOffset = new Vector2(0f, 0.55f);
    #endregion

    #region PrivateVariables
    Animator _animator;
    Collider2D[] _hits;
    EnemyAttack _enemyAttack;
    #endregion

    #region PublicVariables
    public bool IsParried { get; set; }
    #endregion

    #region PrivateMethods
    void Awake()
    {
        _enemyAttack = GetComponentInParent<EnemyAttack>();
    }

    void Start()
    {
        gameObject.SetActive(false);
        _hits = new Collider2D[5];
        IsParried = false;
    }

    void RotateToPlayer()
    {
        if (_enemyAttack != null)
        {
            Vector2 direction = _enemyAttack.GetdirectionToPlayer();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // rotate to player
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // move to player
            transform.localPosition = (Vector3)(direction * _weaponOffset);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_hitTransform.position, _hitRadius);
    }
    #endregion

    #region PublicMethods
    public void StartAttack()
    {
        EnemyAttack enemy = GetComponentInParent<EnemyAttack>();

        if (TryGetComponent<Animator>(out _animator))
        {
            RotateToPlayer();

            _animator.SetTrigger("ShortAttack");
        }
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }

    public void AttackCollideWithPlayer()
    {
        if (IsParried) return;

        Physics2D.OverlapCircleNonAlloc(_hitTransform.position, _hitRadius, _hits, LayerMask.GetMask("Player", "Enemy", "Glass", "Box"));
        IDamage damagable;

        for (int i=0; i<_hits.Length; i++)
        {
            if(_hits[i] != null && _hits[i].gameObject != _enemyAttack.gameObject &&_hits[i].TryGetComponent<IDamage>(out damagable))
            {
                Debug.Log("Damaged!");
                damagable.GetDamaged(transform.position);
            }
        }
    }
    #endregion
}
