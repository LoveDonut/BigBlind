using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using UnityEngine.Windows;

// made by KimDaehui
public class PlayerShortAttack : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Transform _hitTransform;
    [SerializeField] Transform _playerSpriteTransform;
    public AnimationClip ShortAttackAnimationClip;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] Vector2 _hitSize;
    [SerializeField] float _tackleSpeed = 15f;
    [SerializeField] float _shortAttackCoolTime = 1f;

    [Header("Slowmo")]
    [SerializeField] float _slowDownDelay = 0.2f;
    [SerializeField] float _slowDownDuration = 2f;
    [SerializeField] float _slowDownOffset = 0.2f;

    [SerializeField] AudioClip _shortAttackSFX;
    [SerializeField] AudioClip _shortAttackSuccessSFX;


    Coroutine _attackCoroutine;
    Animator _animator;
    PlayerMovement _playerMovement;
    Rigidbody2D _rigidbody;
    #endregion

    #region PublicVariables
//    [HideInInspector] public Vector2 TackleVelocity;
    [HideInInspector] public bool CanAttack;
    public float ShortAttackDistance = 1f;
    #endregion

    #region PrivateMethods

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        CanAttack = true;
        //TackleElapsedTime = _tackleAccelerationTime + _tackleDecelerationTime;
        //if(_shortAttackCoolTime < TackleElapsedTime)
        //{
        //    _shortAttackCoolTime = TackleElapsedTime;
        //}
    }
    void OnShortAttack()
    {
        if (!_playerMovement.IsMovable) return;

        if (_animator != null && _playerMovement != null && CanAttack)
        {
            SoundManager.Instance.PlaySound(_shortAttackSFX, Vector2.zero);
            _playerMovement.CurrentState.SwitchState(gameObject, ref _playerMovement.CurrentState, new ShortAttackState());

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
        CanAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(_hitTransform.position, Quaternion.Euler(0, 0, _playerSpriteTransform.eulerAngles.z), Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, _hitSize);
    }
    #endregion

    #region PublicMethods
    public void CollideWithEnemy()
    {
        Collider2D hit = GetHittedColliderAtBox();
        IDamage damagable;

        // Attack
        if (hit != null && hit.gameObject != gameObject && hit.TryGetComponent<IDamage>(out damagable))
        {
            damagable.GetDamaged((_hitTransform.position - transform.position).normalized);
            SoundManager.Instance.PlaySound(_shortAttackSuccessSFX, Vector2.zero);
        }

        if (hit != null && hit.GetComponent<DoorKick>() != null && !hit.GetComponent<Collider2D>().isTrigger)
        {
            hit.GetComponent<DoorKick>().DoorKicked(transform);
            Invoke("DelayedSlowMotion", _slowDownDelay);
        }

        // Parry
        if(hit != null && hit.gameObject.CompareTag("Attack"))
        {
            Destroy(hit.gameObject);
        }
    }

    void DelayedSlowMotion()
    {
        TimeManager.Instance.DoSlowMotion(_slowDownOffset, _slowDownDuration);
    }

    public void Tackle(Vector2 tackleDirection)
    {
        if (_rigidbody == null) return;

        // set acceleration and decceleration
        //if (TackleElapsedTime > _tackleDecelerationTime)
        //{
        //    TackleVelocity = Vector2.MoveTowards(TackleVelocity, tackleDirection * _maxTackleSpeed, _tackleAcceleration * Time.fixedDeltaTime);
        //}
        //else if (TackleElapsedTime > 0)
        //{
        //    TackleVelocity = Vector2.MoveTowards(TackleVelocity, Vector2.zero, _tackleDeceleration * Time.fixedDeltaTime);
        //}
        //else
        //{
        //    _playerMovement.CurrentState.SwitchState(gameObject, ref _playerMovement.CurrentState, new IdleState());
        //}

        _rigidbody.velocity = _tackleSpeed * tackleDirection;
    }

    public Collider2D GetHittedColliderAtBox() => Physics2D.OverlapBox(_hitTransform.position, _hitSize, _playerSpriteTransform.eulerAngles.z, LayerMask.GetMask("Enemy", "Glass", "Box", "Attack"));
    #endregion
}
