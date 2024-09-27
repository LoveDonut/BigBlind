using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// made by KimDaehui
public class PlayerMovement : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] AudioSource _handCannon;
    [SerializeField] AudioClip _handCannonSound;
    [SerializeField] AudioClip _emptySound;
    [SerializeField] AudioClip _reloadAllSound;

    [SerializeField] AudioClip _cannonOpenCylinder;
    [SerializeField] AudioClip _reloadOneSound;
    [SerializeField] AudioClip _cannonCloseCylinder;

    [SerializeField] GameObject _cameraPos;
    [SerializeField] GameObject _bulletPrefab;

    [SerializeField] GameObject _HandCannonWave;
    #endregion

    #region PrivateVariables
    [Header("Move")]
    [SerializeField] float _acceleration = 50f;    // acceleration
    [SerializeField] float _deceleration = 20f;    // decceleration
    [SerializeField] float _maxSpeed = 5f;         // max move speed

    [Header("SFX")]
    Rigidbody2D _rb;
    Vector2 _input;
    Vector2 _velocity;



    [Header("Bullet")]
    [SerializeField] Color _CannonColor;
    [SerializeField] float _DestroyTime;
    [SerializeField] float _bulletSpeed = 5f;

    [Header("HandCannon")]
    [SerializeField] float _RPM = 60f;
    [SerializeField] int _maxAmmo = 6;
    [SerializeField] float _reloadTime = 2f;
    [SerializeField] bool _reloadAll = true;
    int _ammo = 6;
    bool _isShootable = true, _isReloadable = true;

    Coroutine _reloadCoroutine;
    float _elapsedTime = 0f;
    #endregion

    #region PublicVariables
    public bool IsMovable;
    #endregion

    #region PrivateMethods
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _ammo = _maxAmmo;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
        _rb = GetComponent<Rigidbody2D>();
        IsMovable = true;
    }

    void Update()
    {
        Move();
        if (!_isReloadable)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _reloadTime) _elapsedTime -= _reloadTime;
        }
    }

    void Move()
    {
        if (!IsMovable) return;

        // set acceleration and decceleration
        if (_input.magnitude > 0)
        {
            _velocity += _input * _acceleration * Time.deltaTime;
        }
        else
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, _deceleration * Time.deltaTime);
        }

        // limit max speed
        _velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);
    }

    void OnMove(InputValue value)
    {
        if (!IsMovable) return;

        _input = value.Get<Vector2>();

        _input.Normalize(); // keep the speed in the diagonal direction the same
    }

    void FixedUpdate()
    {
        if (!IsMovable) return;

        // set speed
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    void OnFire(InputValue value)
    {
        if (!IsMovable) return;

        if (_ammo <= 0)
        {
            if(_emptySound != null) _handCannon.PlayOneShot(_emptySound);
            return;
        }
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
        }
        if (!_isReloadable)
        {
            _handCannon.Stop();
            _handCannon.pitch = 1f;
            _isReloadable = true;
        }
        if (!_isShootable)
        {
            return;
        }
        StartCoroutine(WaitNextBullet());
        _ammo--;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
        Direction.Instance.Show_Revolver_Fire_Effect();
        _handCannon.PlayOneShot(_handCannonSound);
        SpawnHandCannonWave();
        CameraShake.instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized, Quaternion.LookRotation(aimPos.normalized));
        bullet.GetComponent<ProjectileMover2D>().aimPos = aimPos;
        Destroy(bullet, 3f);
    }

    IEnumerator WaitNextBullet()
    {
        _isShootable = false;
        yield return new WaitForSeconds(60f / _RPM);
        _isShootable = true;
    }

    void SpawnHandCannonWave()
    {
        var wave = Instantiate(_HandCannonWave, transform.position, Quaternion.identity);

        wave.GetComponent<SoundRayWave>().WaveColor = _CannonColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = _DestroyTime;
    }

    void OnReload(InputValue value)
    {
        if (!IsMovable) return;

        if (_ammo == _maxAmmo || !_isReloadable)
        {
            return;
        }
        _reloadCoroutine = StartCoroutine(WaitReload());
    }

    IEnumerator WaitReload()
    {
        _elapsedTime = 0f;
        _isReloadable = false;
        if (_handCannon.isPlaying) _handCannon.Stop();

        if (!_reloadAll)
        {
            _handCannon.PlayOneShot(_cannonOpenCylinder);
            yield return new WaitForSeconds(.2f);
        }

        while(_ammo < _maxAmmo)
        {
            if (_reloadAll)
            {
                if (_reloadAllSound != null)
                {
                    //HandCannon.pitch = ReloadAllSound.length / reloadTime;
                    _handCannon.PlayOneShot(_reloadAllSound);
                }
            }
            else
            {
                if(_reloadOneSound != null)
                {
                    //HandCannon.pitch = ReloadOneSound.length / reloadTime;
                    _handCannon.PlayOneShot(_reloadOneSound);
                }
            }
            yield return new WaitForSeconds(_reloadAll ? _reloadTime : _reloadTime / 2f);
            if (_reloadAll)
            {
                _ammo = _maxAmmo;
                _handCannon.pitch = 1f;
                Direction.Instance.Show_Revolver_Reload_Effect(true);
            }
            else
            {
                _ammo++;
                Direction.Instance.Sync_BulletCount_UI(_ammo);
                _handCannon.pitch = 1f;
                Direction.Instance.Show_Revolver_Reload_Effect(false);
            }
        }

        if (!_reloadAll)
        {
            _handCannon.PlayOneShot(_reloadOneSound);
            yield return new WaitForSeconds(.2f);
            _handCannon.PlayOneShot(_cannonCloseCylinder);
        }

        _isReloadable = true;
    }
    #endregion

    #region PublicMethods
    #endregion
}
