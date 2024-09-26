using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region PrivateVariables
    [SerializeField] float _acceleration = 50f;    // acceleration
    [SerializeField] float _deceleration = 20f;    // decceleration
    [SerializeField] float _maxSpeed = 5f;         // max move speed
    [SerializeField] float _bulletSpeed = 5f;

    [Header("Sound")]
    [SerializeField] AudioSource HandCannon;
    [SerializeField] AudioClip HandCannonSound;
    [SerializeField] AudioClip EmptySound;
    [SerializeField] AudioClip ReloadAllSound;
    [SerializeField] AudioClip ReloadOneSound;
    Rigidbody2D _rb;
    Vector2 _input;
    Vector2 _velocity;
    bool _isMovable = true;

    [SerializeField] GameObject _cameraPos;
    [SerializeField] GameObject _bulletPrefab;

    [Header("Bullet")]
    [SerializeField] GameObject _HandCannonWave;
    [SerializeField] Color CannonColor;
    [SerializeField] float Destroy_Time;

    [Header("HandCannon")]
    [SerializeField] float RPM = 60f;
    [SerializeField] int _maxAmmo = 6;
    int _ammo = 6;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] bool reloadAll = true;
    bool shootable = true, reloadable = true;
    [SerializeField] TMPro.TextMeshProUGUI AmmoCount;
    Coroutine reloadCoroutine;
    #endregion

    #region PrivateMethods
    void Start()
    {

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _ammo = _maxAmmo;
        _rb = GetComponent<Rigidbody2D>();
        if (AmmoCount != null) AmmoCount = GameObject.Find("AmmoCount").GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        Move();
        AmmoCount.text = _ammo + " / " + _maxAmmo;
    }

    void Move()
    {
        if (!_isMovable) return;

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
        _input = value.Get<Vector2>();

        _input.Normalize(); // keep the speed in the diagonal direction the same
    }

    void FixedUpdate()
    {
        if (!_isMovable) return;

        // set speed
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    void OnFire(InputValue value)
    {
        if (_ammo <= 0)
        {
            if(EmptySound != null) HandCannon.PlayOneShot(EmptySound);
            return;
        }
        if(reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadable = true;
        if (!shootable)
        {
            return;
        }
        StartCoroutine(WaitNextBullet());
        _ammo--;
        HandCannon.PlayOneShot(HandCannonSound);
        SpawnHandCannonWave();
        CameraShake.instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized, Quaternion.LookRotation(aimPos.normalized));

        Destroy(bullet, 3f);
    }

    IEnumerator WaitNextBullet()
    {
        shootable = false;
        yield return new WaitForSeconds(60f / RPM);
        shootable = true;
    }

    void SpawnHandCannonWave()
    {
        var wave = Instantiate(_HandCannonWave, transform.position, Quaternion.identity);

        wave.GetComponent<SoundRayWave>().WaveColor = CannonColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = Destroy_Time;
    }

    void OnReload(InputValue value)
    {
        if(_ammo == _maxAmmo || !reloadable)
        {
            return;
        }
        reloadCoroutine = StartCoroutine(WaitReload());
    }

    IEnumerator WaitReload()
    {
        reloadable = false;
        while(_ammo < _maxAmmo)
        {
            if (reloadAll)
            {
                if (ReloadAllSound != null)
                {
                    HandCannon.PlayOneShot(ReloadAllSound);
                }
            }
            else
            {
                if(ReloadOneSound != null)
                {
                    HandCannon.PlayOneShot(ReloadOneSound);
                }
            }
            yield return new WaitForSeconds(reloadTime);
            if (reloadAll)
            {
                _ammo = _maxAmmo;
            }
            else
            {
                _ammo++;
            }
        }
        reloadable = true;
    }
    #endregion

    #region PublicMethods
    #endregion
}
