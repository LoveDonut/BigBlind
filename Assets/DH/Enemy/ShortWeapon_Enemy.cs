using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortWeapon_Enemy : MonoBehaviour
{
    [SerializeField] Transform _hitTransform;
    [SerializeField] float _hitRadius = 0.5f;
    [SerializeField] Vector2 _weaponOffset = new Vector2(0f,0.55f);

    Animator _animator;


    #region PrivateMethods
    void Start()
    {
        gameObject.SetActive(false);
    }

    void RotateToPlayer()
    {
        Enemy enemy = GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Vector2 direction = enemy.GetdirectionToPlayer();

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
        Enemy enemy = GetComponentInParent<Enemy>();

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
        Collider2D hit = Physics2D.OverlapCircle(_hitTransform.position, _hitRadius, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            PlayerHealth playerHealth;

            if(hit.gameObject.TryGetComponent<PlayerHealth>(out playerHealth))
            {
                playerHealth.GetDamaged(transform.position);
            }
        }
    }
    #endregion
}
