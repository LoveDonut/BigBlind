using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerState;
using UnityEngine.UI;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    #region References
    [Header("References")]
    public Weapon RevolverData;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _handCannonWave;
    #endregion

    #region PrivateVariables
    [Header("Weapon")]
    [SerializeField] string _currentWeapon = "Revolver";
    [SerializeField] float _RPM = 200f;
    [SerializeField] bool _automatic = false;
    [SerializeField] int _maxAmmo = 6;
    [SerializeField] int _reserveAmmo = 30;
    [SerializeField] bool _infiniteReserve = false;
    [SerializeField] float _reloadTime = 1f;
    [SerializeField] bool _reloadAll = false;
    int _ammo = 6;
    bool _isShootable = true, _isReloadable = false, _isReloading = false;
    float _localMult = 1f;
    public bool _isFiring = false;

    [Header("Bullet")]
    [SerializeField] Color _cannonColor;
    [SerializeField] float _destroyTime;
    [SerializeField] float _bulletSpeed = 60f;

    [Header("UI")]
    [SerializeField] Image _ammoUI;
    [SerializeField] TMP_Text _leftAmmoText;
    [SerializeField] TMP_Text _leftAmmoTextShadow;


    [Header("SFX")]
    [SerializeField] AudioSource _handCannon;

    [SerializeField] AudioClip _handCannonSound;
    [SerializeField] AudioClip _emptySound;
    [SerializeField] AudioClip _reloadAllSound;

    [SerializeField] AudioClip _OpenCylinder;
    [SerializeField] AudioClip _reloadOneSound;
    [SerializeField] AudioClip _closeCylinder;

    PlayerMovement _playerMovement;
    Coroutine _reloadCoroutine;
    float _elapsedTime = 0f, _reloadingTime = 0f;
    private float _velocity;

    Weapon _currentHoldWeapon;
    #endregion

    #region PublicVariables
    [HideInInspector] public bool IsHaste = false;
    [Header("Buffer")]
    public float BufferDuration = 0.2f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _currentWeapon = "Revolver";
        _currentHoldWeapon = RevolverData;
        _ammo = _maxAmmo;
        _isFiring = false;
        _localMult = 1f;
        InvokeRepeating(nameof(startCheckReloadable), 0, 30 / (GetComponent<WaveManager>().BPM * 8));
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isReloadable)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _reloadTime / _localMult) _elapsedTime -= _reloadTime / _localMult;
        }

        if(_isFiring && _isShootable)
        {
            Shoot();
        }

        if (_isReloading)
        {
            _reloadingTime += Time.deltaTime;
            _ammoUI.fillAmount = Mathf.Lerp(_ammoUI.fillAmount, _reloadingTime / _reloadTime, 20 * Time.deltaTime);
            _leftAmmoText.text = _leftAmmoTextShadow.text = "-";
        }
        else
        {
            float target = (float)_ammo / _currentHoldWeapon.Ammo;
            _ammoUI.fillAmount = Mathf.SmoothDamp(_ammoUI.fillAmount, target, ref _velocity, 0.1f);
            _leftAmmoText.text = _leftAmmoTextShadow.text = _ammo.ToString();
        }
    }

    void startCheckReloadable() => _isReloadable = !_isReloadable;

    void OnFire(InputValue value)
    {
        if(TryGetComponent<PlayerMovement>(out _playerMovement))
        {
            if(!_playerMovement.IsMovable) return;
        }
        if (value.isPressed)
        {
            if (_ammo <= 0)
            {
                // Reload when player tries to fire when mag is empty
                if(_reloadCoroutine == null) _reloadCoroutine = StartCoroutine(WaitReload());
                return;
            }
            _isFiring = true;
            if (_reloadCoroutine != null)
            {
                _isReloading = false;
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
                SoundManager.Instance.ReloadAudio.Stop();
            }
        }
        else
        {
            _isFiring = false;
            return;
        }
        if (!GetComponent<PlayerMovement>().IsMovable || !_isShootable) return;

        // save OnFire input if made on buffer time
        if(GetComponent<PlayerMovement>().CurrentState.GetType() == typeof(ShortAttackState))
        {
            ShortAttackState shortAttackState = GetComponent<PlayerMovement>().CurrentState as ShortAttackState;
            shortAttackState.IsClickedOnBuffer = shortAttackState.IsBufferTime ? true : false;

            return;
        }

        Shoot();
    }
    IEnumerator WaitNextBullet()
    {
        _isShootable = false;
        yield return new WaitForSeconds(60f / _RPM / _localMult);
        _isShootable = true;
    }

    void SpawnHandCannonWave()
    {
        var wave = Instantiate(_handCannonWave, transform.position, Quaternion.identity);
        wave.GetComponent<SoundRayWave>().isCannonWave = true;
        wave.GetComponent<SoundRayWave>().WaveColor = _cannonColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = _destroyTime;
    }

    void OnReload(InputValue value)
    {
        if (!GetComponent<PlayerMovement>().IsMovable || _reserveAmmo <= 0 ||
            GetComponent<PlayerMovement>().CurrentState.GetType() == typeof(ShortAttackState) || _currentWeapon.CompareTo("Revolver") != 0) return;

        if (_ammo >= _maxAmmo || _isReloading)
        {
            return;
        }
        _isFiring = false;
        _reloadCoroutine = StartCoroutine(WaitReload());
    }

    IEnumerator WaitReload()
    {
        yield return new WaitUntil(() => _isReloadable);
        _isReloading = true;
        _elapsedTime = 0f;

        if (!_reloadAll)
        {
            SoundManager.Instance.PlaySound(_OpenCylinder, Vector2.zero);

            yield return new WaitForSeconds(_reloadTime / _localMult / 2);
        }

        while (_ammo < _maxAmmo && _reserveAmmo > 0)
        {
            if (_reloadAll)
            {
                SoundManager.Instance.ReloadAudio.pitch = _reloadAllSound.length / _reloadTime * _localMult;
                SoundManager.Instance.ReloadAudio.Play();
            }
            else
            {
                SoundManager.Instance.PlaySound(_reloadOneSound, Vector2.zero);
            }
            yield return new WaitForSeconds(_reloadAll ? _reloadTime / _localMult : _reloadTime / _localMult / 2f);
            if (_reloadAll)
            {
                if (_reloadAllSound != null)
                {
                    if(_reserveAmmo < (_maxAmmo - _ammo))
                    {
                        if (!_infiniteReserve) _reserveAmmo = 0;
                        _ammo += _reserveAmmo;
                    }
                    else
                    {
                        if(!_infiniteReserve) _reserveAmmo -= (_maxAmmo - _ammo);
                        _ammo = _maxAmmo;
                    }

                    //HandCannon.pitch = ReloadAllSound.length / reloadTime;

                }
            }
            else
            {
                if (_reloadOneSound != null)
                {
                    if(!_infiniteReserve) _reserveAmmo--;
                    _ammo++;

                }
            }
        }

        if (!_reloadAll) SoundManager.Instance.PlaySound(_closeCylinder, Vector2.zero);


        _isReloading = false;
        _reloadingTime = 0;
    }
    public void Shoot()
    {
        if (_ammo <= 0)
        {
            return;
        }
        _leftAmmoTextShadow.transform.parent.GetComponent<Animator>().Play("Bullet_Shoot");


        StartCoroutine(WaitNextBullet());
        _ammo--;
        if (!_automatic) _isFiring = false;
       

        SoundManager.Instance.PlaySound(_handCannonSound, Vector2.zero);

        SpawnHandCannonWave();
        CameraShake.Instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized * 1.5f, Quaternion.LookRotation(aimPos.normalized));
        if(bullet.GetComponent<ProjectileMover2D>() != null)
        {
            bullet.GetComponent<ProjectileMover2D>().Speed = _bulletSpeed;
            bullet.GetComponent<ProjectileMover2D>().AimPos = aimPos;
            bullet.GetComponent<ProjectileMover2D>().IsFromPlayer = true;
            bullet.GetComponent<ProjectileMover2D>().FromObject = gameObject;
        }
        List<Transform> children = new List<Transform>();
        foreach(Transform child in bullet.transform)
        {
            if(child.GetComponent<ProjectileMover2D>() != null)
            {
                children.Add(child);
                child.GetComponent<ProjectileMover2D>().Speed = _bulletSpeed;
                child.GetComponent<ProjectileMover2D>().AimPos = aimPos;
                child.GetComponent<ProjectileMover2D>().IsFromPlayer = true;
                child.GetComponent<ProjectileMover2D>().FromObject = gameObject;
            }
        }
        foreach(Transform child in children)
        {
            child.parent = null;
        }
        Destroy(bullet, 3f);

        if(_currentWeapon.CompareTo("Revolver") != 0)
        {
            if (_ammo <= 0) ReturnToRevolver();
        }
    }
    public void AddReserveAmmo(int count)
    {
        _reserveAmmo += count;
    }

    public IEnumerator Haste(float mult, float duration)
    {
        IsHaste = true;
        _localMult = mult;
        yield return new WaitForSeconds(duration);
        _localMult = 1f;
        IsHaste = false;
    }

    public void ChangeWeapon(Weapon weapon)
    {
        if (_reloadCoroutine != null)
        {
            _isReloading = false;
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }
        _currentHoldWeapon = weapon;
        _isFiring = false;
        RevolverData.Ammo = _ammo;
        _currentWeapon = weapon.WeaponName;
        _bulletPrefab = weapon.BulletPrefab;
        _handCannonWave = weapon.WavePrefab;
        _RPM = weapon.RPM;
        _automatic = weapon.Automatic;
        _ammo = weapon.Ammo;
        _bulletSpeed = weapon.BulletSpeed;
        _handCannonSound = weapon.FireSound;
        _cannonColor = weapon.WaveColor;
        _destroyTime = weapon.DestroyTime;
    }

    public void ReturnToRevolver()
    {
        _currentHoldWeapon = RevolverData;
        _currentHoldWeapon.Ammo = _maxAmmo;
        _isFiring = false;
        _ammo = RevolverData.Ammo;
        _currentWeapon = RevolverData.WeaponName;
        _bulletPrefab = RevolverData.BulletPrefab;
        _handCannonWave = RevolverData.WavePrefab;
        _RPM = RevolverData.RPM;
        _automatic = RevolverData.Automatic;
        _bulletSpeed = RevolverData.BulletSpeed;
        _handCannonSound = RevolverData.FireSound;
        _cannonColor = RevolverData.WaveColor;
        _destroyTime = RevolverData.DestroyTime;
    }
}
