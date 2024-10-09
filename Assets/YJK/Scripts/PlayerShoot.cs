using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().Play();
        _ammo = _maxAmmo;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
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

        if (_ammo <= 0)
        {
            if (_emptySound != null) _handCannon.PlayOneShot(_emptySound);
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
        _handCannon.PlayOneShot(_handCannonSound);
        SpawnHandCannonWave();
        CameraShake.Instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized * 0.6f, Quaternion.LookRotation(aimPos.normalized));
        bullet.GetComponent<ProjectileMover2D>().speed = _bulletSpeed;
        bullet.GetComponent<ProjectileMover2D>().aimPos = aimPos;
        Destroy(bullet, 3f);
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

        wave.GetComponent<SoundRayWave>().WaveColor = _cannonColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = _destroyTime;
    }

    void OnReload(InputValue value)
    {
        if (!GetComponent<PlayerMovement>().IsMovable || _reserveAmmo <= 0) return;

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
            _handCannon.PlayOneShot(_OpenCylinder);

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
                    _handCannon.PlayOneShot(_reloadAllSound);
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
                    _handCannon.PlayOneShot(_reloadOneSound);
                    Direction.Instance.Show_Revolver_Reload_Effect(false);

                }
            }
            yield return new WaitForSeconds(_reloadAll ? _reloadTime : _reloadTime / 2f);
        }

        if (!_reloadAll) _handCannon.PlayOneShot(_closeCylinder);

        _isReloading = false;
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
