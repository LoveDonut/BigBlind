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
    [SerializeField] float _reloadTime = 1f;
    [SerializeField] bool _reloadAll = false;
    int _ammo = 6;
    bool _isShootable = true, _isReloadable = true;

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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _ammo = _maxAmmo;
        Direction.Instance.Sync_BulletCount_UI(_ammo);
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

    void OnFire(InputValue value)
    {
        if (!GetComponent<PlayerMovement>().IsMovable) return;

        if(_ammo <= 0)
        {
            if (_emptySound != null) _handCannon.PlayOneShot(_emptySound);
            return;
        }
        if(_reloadCoroutine != null)
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
        CameraShake.Instance.shakeCamera(7f, .1f);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + aimPos.normalized, Quaternion.LookRotation(aimPos.normalized));
        bullet.GetComponent<ProjectileMover2D>().speed = _bulletSpeed;
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
        var wave = Instantiate(_handCannonWave, transform.position, Quaternion.identity);

        wave.GetComponent<SoundRayWave>().WaveColor = _cannonColor;
        wave.GetComponent<SoundRayWave>().InitWave();
        wave.GetComponent<SoundRayWave>().Destroy_Time = _destroyTime;
    }

    void OnReload(InputValue value)
    {
        if (!GetComponent<PlayerMovement>().IsMovable) return;

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
        //if (_handCannon.isPlaying) _handCannon.Stop();

        if (!_reloadAll)
        {
            _handCannon.PlayOneShot(_OpenCylinder);

            yield return new WaitForSeconds(.2f);
        }

        while (_ammo < _maxAmmo)
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
                if (_reloadOneSound != null)
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
            _handCannon.PlayOneShot(_closeCylinder);
        }

        _isReloadable = true;
    }
}
