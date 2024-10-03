using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public class PlayerShortAttack : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] GameObject _weapon;
    [SerializeField] Animator _shortWeaponAnimator;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] float _shortAttackCoolTime = 1f;
    [SerializeField] Vector2 _weaponOffset = new Vector2(0f, 0.55f);

    bool _canAttack;
    Coroutine _attackCoroutine;
    #endregion

    #region PrivateMethods

    void Start()
    {
        _weapon.gameObject.SetActive(false);
        _canAttack = true;
    }

    // rotate and move towards mouse, because player does not rotate
    void RotateToMouse()
    {
        if (_weapon != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // rotate to player
            _weapon.transform.rotation = Quaternion.Euler(0, 0, angle);

            // move to player
            _weapon.transform.localPosition = (Vector3)(direction * _weaponOffset);
        }
    }
    void OnShortAttack()
    {
        if (_shortWeaponAnimator != null && _canAttack)
        {
            _canAttack = false;
            _weapon.SetActive(true);
            RotateToMouse();
            _shortWeaponAnimator.SetTrigger("ShortAttack");

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

    #endregion

    #region PublicMethods
    #endregion
}
