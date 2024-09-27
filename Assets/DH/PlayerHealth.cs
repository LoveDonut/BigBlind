using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
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
    int _currentHp;
    float _originalAlpha;

    #region PriavteMethods

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Start()
    {
        _isInvincible = false;
        _currentHp = _maxHp;
        _originalAlpha = _spriteRenderer.color.a;
    }

    void Dead()
    {
        _playerMovement._isMovable = false;
        Debug.Log("Dead!");
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

        PlayerMovement playerMovement;

        if (TryGetComponent<PlayerMovement>(out playerMovement))
        {
            
        }
        DoKnockBack(_slowDownDuration, attackedDirection);

        // shake camera
        CameraShake.Instance.shakeCamera(_cameraShakeIntensity, _slowDownDuration);

        // slow down during get damaged
        TimeManager.instance.DoSlowMotion(_slowDownOffset, _slowDownDuration);


        // be invincible for a while
        MakeInvincible(_damagedDuration, true);

        // Update Hp
        _currentHp -= damage;
        _playerMovement.HeartBeat.volume += .25f;
        _playerMovement.Beat.volume -= .35f;

        if (_currentHp <= 1) Direction.Instance.ShowLowHP();

        // TODO : Update HpUI
        //if (_healthUI != null)
        //{
        //    _healthUI.UpdateHealthUI(Hp);
        //}
        //else
        //{
        //    Debug.Log("Can't find HeartUI Script...");
        //}

        if (_currentHp <= 0)
        {
            Dead();
        }
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
            _playerMovement._isMovable = false;
        }

        float elapsedTime = duration;

        Vector2 playerVelocity = knockBackDirection * _knockbackSpeed;
        while (elapsedTime > 0)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);

            if (rb != null)
            {
                rb.MovePosition(rb.position + playerVelocity * Time.fixedDeltaTime);
            }
            elapsedTime -= Time.fixedDeltaTime;

            if (Time.timeScale >= 1f)
            {
                break;
            }
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        if (_playerMovement != null && _currentHp > 0)
        {
            _playerMovement._isMovable = true;
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
