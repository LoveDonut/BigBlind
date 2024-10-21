using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerState;

public class PlayerShoot : MonoBehaviour
{
    #region References
    [Header("References")]
    public Weapon RevolverData;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _handCannonWave;
    #endregion

    #region PrivateVariables
    [Header("BeatTest")]
    public int ShootBPMMultiplier = 2;
    [HideInInspector] public int ShootCount;

    [Header("HandCannon")]
    [SerializeField] string _currentWeapon = "Revolver";
    [SerializeField] float _RPM = 200f;
    [SerializeField] int _maxAmmo = 6;
    [SerializeField] int _reserveAmmo = 30;
    [SerializeField] bool _infiniteReserve = false;
    [SerializeField] float _reloadTime = 1f;
    [SerializeField] bool _reloadAll = false;
    int _ammo = 6;
    bool _isReloadable = false, _isReloading = false;
    [SerializeField] bool _isFiring = false;

    [Header("Bullet")]
    [SerializeField] Color _cannonColor;
    [SerializeField] float _destroyTime;
    [SerializeField] float _bulletSpeed = 60f;

    [Header("SFX")]
    [SerializeField] AudioSource _handCannon;

    [SerializeField] AudioClip _handCannonSound;
    [SerializeField] AudioClip _emptySound;
    [SerializeField] AudioClip _reloadAllSound;

    [SerializeField] AudioClip _OpenCylinder;
    [SerializeField] AudioClip _reloadOneSound;
    [SerializeField] AudioClip _closeCylinder;

    Coroutine _reloadCoroutine;
    float _elapsedTime = 0f;
    #endregion

    #region PublicVariables
    [HideInInspector] public bool IsHaste = false;
    [Header("Buffer")]
    public float BufferDuration = 0.2f;
    public bool IsShootable;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _currentWeapon = "Revolver";
        _ammo = _maxAmmo;
        _isFiring = false;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
        InvokeRepeating(nameof(startCheckReloadable), 0, 30 / (GetComponent<WaveManager>().BPM * 8));

        ShootCount = ShootBPMMuitiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isReloadable)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _reloadTime) _elapsedTime -= _reloadTime;
        }

    }

    void startCheckReloadable() => _isReloadable = !_isReloadable;

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            _isFiring = true;
            if(_ammo <= 0 && _emptySound != null) SoundManager.Instance.PlaySound(_emptySound, Vector2.zero);
            if (_reloadCoroutine != null)
            {
                _isReloading = false;
                StopCoroutine(_reloadCoroutine);
            }
            IsShootable = true;
        }
        else
        {
            _isFiring = false;
        }
        if (!GetComponent<PlayerMovement>().IsMovable) return;


        // save OnFire input if made on buffer time
        if(GetComponent<PlayerMovement>().CurrentState.GetType() == typeof(ShortAttackState))
        {
            ShortAttackState shortAttackState = GetComponent<PlayerMovement>().CurrentState as ShortAttackState;
            shortAttackState.IsClickedOnBuffer = shortAttackState.IsBufferTime ? true : false;

            return;
        }

//        Shoot();
    }
    IEnumerator WaitNextBullet()
    {
        yield return new WaitForSecondsRealtime(60f / _RPM);
        IsShootable = true;
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

        if (_ammo == _maxAmmo || _isReloading)
        {
            return;
        }
        _reloadCoroutine = StartCoroutine(WaitReload());
    }

    IEnumerator WaitReload()
    {
        yield return new WaitUntil(() => _isReloadable);
        _isReloading = true;
        _elapsedTime = 0f;
        //if (_handCannon.isPlaying) _handCannon.Stop();

        if (!_reloadAll)
        {
            SoundManager.Instance.PlaySound(_OpenCylinder, Vector2.zero);

            yield return new WaitForSeconds(_reloadTime / 2);
        }

        while (_ammo < _maxAmmo && _reserveAmmo > 0)
        {
            if (_reloadAll)
            {
                if (_reloadAllSound != null)
                {
                    if(_reserveAmmo < (_maxAmmo - _ammo))
                    {
                        if (!_infiniteReserve) _reserveAmmo = 0;
                        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
                        _ammo += _reserveAmmo;
                    }
                    else
                    {
                        if(!_infiniteReserve) _reserveAmmo -= (_maxAmmo - _ammo);
                        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
                        _ammo = _maxAmmo;
                    }

                    //HandCannon.pitch = ReloadAllSound.length / reloadTime;
                    SoundManager.Instance.PlaySound(_reloadAllSound, Vector2.zero);
                    Direction.Instance.Show_Revolver_Reload_Effect(true);

                }
            }
            else
            {
                if (_reloadOneSound != null)
                {
                    if(!_infiniteReserve) _reserveAmmo--;
                    Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
                    _ammo++;
                    Direction.Instance.Sync_BulletCount_UI(_ammo);

                    //HandCannon.pitch = ReloadOneSound.length / reloadTime;
                    SoundManager.Instance.PlaySound(_reloadOneSound, Vector2.zero);
                    Direction.Instance.Show_Revolver_Reload_Effect(false);

                }
            }
            yield return new WaitForSeconds(_reloadAll ? _reloadTime : _reloadTime / 2f);
        }

        if (!_reloadAll) SoundManager.Instance.PlaySound(_closeCylinder, Vector2.zero);


        _isReloading = false;
    }
    public void Shoot()
    {
        if (_ammo <= 0)
        {
            return;
        }
        
//        StartCoroutine(WaitNextBullet());
        _ammo--;
        
        if(_currentWeapon.CompareTo("Revolver") == 0)
        {
            Direction.Instance.Sync_BulletCount_UI(_ammo);
            Direction.Instance.Show_Revolver_Fire_Effect();
        }

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
            else Direction.Instance.SyncReserveAmmoUI(_ammo);
        }
    }
    public void AddReserveAmmo(int count)
    {
        _reserveAmmo += count;
        if(_currentWeapon.CompareTo("Revolver") == 0) Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
    }

    public IEnumerator Haste(float mult, float duration)
    {
        IsHaste = true;
        _RPM *= mult;
        _reloadTime /= mult;
        yield return new WaitForSeconds(duration);
        _RPM /= mult;
        _reloadTime *= mult;
        IsHaste = false;
    }

    public void ChangeWeapon(Weapon weapon)
    {
        if (_reloadCoroutine != null)
        {
            _isReloading = false;
            StopCoroutine(_reloadCoroutine);
        }
        RevolverData.Ammo = _ammo;
        _currentWeapon = weapon.WeaponName;
        _bulletPrefab = weapon.BulletPrefab;
        _handCannonWave = weapon.WavePrefab;
        _RPM = weapon.RPM;
        _ammo = weapon.Ammo;
        _bulletSpeed = weapon.BulletSpeed;
        _handCannonSound = weapon.FireSound;
        _cannonColor = weapon.WaveColor;
        _destroyTime = weapon.DestroyTime;
        Direction.Instance.SyncReserveAmmoUI(_ammo);
        Direction.Instance.SyncBulletImage(weapon.BulletImage);
    }

    public void ReturnToRevolver()
    {
        _ammo = RevolverData.Ammo;
        _currentWeapon = RevolverData.WeaponName;
        _bulletPrefab = RevolverData.BulletPrefab;
        _handCannonWave = RevolverData.WavePrefab;
        _RPM = RevolverData.RPM;
        _ammo = RevolverData.Ammo;
        _bulletSpeed = RevolverData.BulletSpeed;
        _handCannonSound = RevolverData.FireSound;
        _cannonColor = RevolverData.WaveColor;
        _destroyTime = RevolverData.DestroyTime;
        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
        Direction.Instance.SyncBulletImage(RevolverData.BulletImage);
    }
}
