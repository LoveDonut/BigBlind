using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;

// made by KimDaehui
public class PlayerHealth : MonoBehaviour, IDamage
{
    [SerializeField] float _damagedDuration = 2f;
    [SerializeField] float _blinkCycle = 0.4f;
    [SerializeField] float _slowDownDuration = 0.5f;
    [SerializeField] float _slowDownOffset = 0.2f;
    [SerializeField] float _knockbackSpeed = 1f;
    [SerializeField] float _cameraShakeIntensity = 2f;
    [SerializeField] int _maxHp = 3;

    PlayerMovement _playerMovement;
    SpriteRenderer _spriteRenderer;
    Coroutine _invincibleCoroutine;
    bool _isInvincible;
    float _originalAlpha;

    //made by Chun Jin Ha
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public bool IsFullHealth => CurrentHp == MaxHp;
    #region PriavteMethods

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Start()
    {
        _isInvincible = false;
        CurrentHp = MaxHp = _maxHp;
        _originalAlpha = _spriteRenderer.color.a;
    }

    IEnumerator ResetInvincible(float invincibleDuration, bool isBlink)
    {
        float elapsedTime = invincibleDuration;

        // blink player during invincible
        while (elapsedTime > 0f)
        {
            if (_spriteRenderer != null && isBlink)
            {
                float changedAlpha = _spriteRenderer.color.a == _originalAlpha ? _spriteRenderer.color.a / 2f : _originalAlpha;
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, changedAlpha);
            }

            yield return new WaitForSeconds(_blinkCycle);
            //            yield return new WaitForSeconds(_blinkCycle * Time.timeScale); // blink cycle is in realtime

            elapsedTime -= _blinkCycle;
            //            elapsedTime -= _blinkCycle * Time.timeScale; // blink cycle is in realtime
        }

        if (_spriteRenderer != null && isBlink)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _originalAlpha);
        }

        _isInvincible = false;
    }
    #endregion


    #region PublicMethods
    public void GetDamaged(Vector2 attackedDirection, int damage = 1)
    {
        if (_isInvincible) return;

        PlayerShortAttack playerShortAttack;
        if (TryGetComponent<PlayerShortAttack>(out playerShortAttack))
        {
            Collider2D hit = playerShortAttack.GetHittedColliderAtBox();
            if (hit != null && hit.CompareTag("Attack"))
            {
                return;
            }
        }

        // do not knockback when tackle
        if (_playerMovement.CurrentState.GetType() != typeof(ShortAttackState))
        {
            DoKnockBack(_slowDownDuration, attackedDirection);
        }

        // shake camera
        CameraShake.Instance.shakeCamera(_cameraShakeIntensity, _slowDownDuration);

        // slow down during get damaged
        TimeManager.Instance.DoSlowMotion(_slowDownOffset, _slowDownDuration);


        // be invincible for a while
        MakeInvincible(_damagedDuration, true);

        // Update Hp
        CurrentHp -= damage;

        // TODO : Update HpUI
        //if (_healthUI != null)
        //{
        //    _healthUI.UpdateHealthUI(Hp);
        //}
        //else
        //{
        //    Debug.Log("Can't find HeartUI Script...");
        //}

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Dead();
        }

        _playerMovement.HeartBeat.volume += 1f / MaxHp;
        _playerMovement.Beat.volume -= 1f / MaxHp;

        if (CurrentHp <= 1) Direction.Instance.ShowLowHP();
    }

    public void Dead()
    {
        Direction.Instance.ShowGameOver();
        _playerMovement.IsMovable = false;
    }
    public void DoKnockBack(float duration, Vector2 knockBackDirection)
    {
        StartCoroutine(KnockBack(duration, knockBackDirection));
    }

    IEnumerator KnockBack(float duration, Vector2 knockBackDirection)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // can't move during get damaged
        if (_playerMovement != null)
        {
            _playerMovement.IsMovable = false;
        }

        float elapsedTime = duration;

        Vector2 playerVelocity = knockBackDirection * _knockbackSpeed;
        while (elapsedTime > 0 && Time.timeScale < 1f)
        {
            yield return new WaitForSecondsRealtime(Time.deltaTime);

            if (rb != null)
            {
                rb.velocity = playerVelocity;
            }
            elapsedTime -= Time.deltaTime;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        if (_playerMovement != null && CurrentHp > 0)
        {
            _playerMovement.IsMovable = true;
        }
    }

    public void MakeInvincible(float invincibleDuration, bool isBlink = false)
    {
        _isInvincible = true;

        if (_invincibleCoroutine != null)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _originalAlpha);
            StopCoroutine(_invincibleCoroutine);
        }

        _invincibleCoroutine = StartCoroutine(ResetInvincible(invincibleDuration, isBlink));
    }
    #endregion
}
