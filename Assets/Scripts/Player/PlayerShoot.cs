using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerState;

public class PlayerShoot : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _handCannonWave;
    #endregion

    #region PrivateVariables
    [Header("HandCannon")]
    [SerializeField] float _RPM = 200f;
    [SerializeField] int _maxAmmo = 6;
    [SerializeField] int _reserveAmmo = 30;
    [SerializeField] bool _infiniteReserve = false;
    [SerializeField] float _reloadTime = 1f;
    [SerializeField] bool _reloadAll = false;
    int _ammo = 6;
    bool _isShootable = true, _isReloadable = false, _isReloading = false;

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

    public bool IsHaste = false;

    #endregion

    #region PublicVariables
    [Header("Buffer")]
    public float BufferDuration = 0.2f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _ammo = _maxAmmo;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
        InvokeRepeating(nameof(startCheckReloadable), 0, 30 / (GetComponent<WaveManager>().BPM * 8));

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
        yield return new WaitForSecondsRealtime(60f / _RPM);
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
            GetComponent<PlayerMovement>().CurrentState.GetType() == typeof(ShortAttackState)) return;

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
            if (_emptySound != null) SoundManager.Instance.PlaySound(_emptySound, Vector2.zero);

            return;
        }
        if (_reloadCoroutine != null)
        {
            _isReloading = false;
            StopCoroutine(_reloadCoroutine);
        }
        StartCoroutine(WaitNextBullet());
        _ammo--;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
        Direction.Instance.Show_Revolver_Fire_Effect();

        SoundManager.Instance.PlaySound(_handCannonSound, Vector2.zero);

        SpawnHandCannonWave();
        CameraShake.Instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized * 0.6f, Quaternion.LookRotation(aimPos.normalized));
        bullet.GetComponent<ProjectileMover2D>().Speed = _bulletSpeed;
        bullet.GetComponent<ProjectileMover2D>().AimPos = aimPos;

        // for preventing attack player self
        bullet.GetComponent<ProjectileMover2D>().IsFromPlayer = true;
        Destroy(bullet, 3f);
    }
    public void AddReserveAmmo(int count)
    {
        _reserveAmmo += count;
        Direction.Instance.SyncReserveAmmoUI(_reserveAmmo);
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
}