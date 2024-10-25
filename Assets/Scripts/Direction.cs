using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Direction : MonoBehaviour
{
    public static Direction Instance { get; private set; }

    [Header("Direction_UI")]
    [SerializeField] GameObject _flash;
    [SerializeField] GameObject _gameOverPanel;

    Canvas _canvas;
    public RectTransform _crossHair;
    [SerializeField] float _smoothness = 0.1f;

    [Header("Ammo_UI")]
    [SerializeField] TextMeshProUGUI _reserveAmmoUI;
    [SerializeField] Image _bulletImage;

    [Header("LowBlood_UI")]
    [SerializeField] Image _lowHpImage;

    [Header("FeverTime")]
    [SerializeField] GameObject _audioSpectrum;
    Material _mat;

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _crossHair.position = Vector2.Lerp(_crossHair.position, mousePosition, _smoothness);
        }
    }

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    #region Shoot
    public void Show_Flash_Effect()
    {
        _flash.GetComponent<Animator>().Play("Flash");
    }
    #endregion

    #region RevolverUI

    public void SyncReserveAmmoUI(int ammo) => _reserveAmmoUI.text = ammo.ToString();
    public void SyncReserveAmmoUI(float ammo) => _reserveAmmoUI.text = ammo.ToString();
    public void SyncBulletImage(Weapon weapon) {
        _bulletImage.GetComponent<Image>().sprite = weapon.BulletImage;
        _bulletImage.GetComponent<Image>().color = weapon.WeaponColor;
        _reserveAmmoUI.color = weapon.WeaponColor;
    }

    #endregion

    #region GameOver

    public void ShowLowHP() => _lowHpImage.GetComponent<Animator>().Play("LowHP");


    public void ShowGameOver() => _gameOverPanel.SetActive(true);


    public void RetryClick() {
        DOTween.KillAll();

        if (LineRendererPool.Instance != null)
        {
            LineRendererPool.Instance.ClearPool();
            Destroy(LineRendererPool.Instance.gameObject);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.RestartStage();

    }
    public void ExitClick() => Application.Quit();

    #endregion

    #region FeverTime

    public void ShowAudioSpectrum()
    {
        _audioSpectrum.SetActive(true);
        _mat = _audioSpectrum.GetComponent<Material>();
    }

    public void SetSpectrumAlphaValue(float value) => _mat.SetFloat("_AlphaValue", value);

    public void HideAudioSpectrum()
    {
        _audioSpectrum.SetActive(false);
    }

    #endregion
}
