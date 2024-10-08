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
    [SerializeField] RectTransform _crossHair;
    [SerializeField] float _smoothness = 0.1f;

    [Header("Revolver_UI")]
    [SerializeField] Revolver_UI _revolverUI;
    [SerializeField] TextMeshProUGUI _reserveAmmoUI;

    [Header("LowBlood_UI")]
    [SerializeField] Image _lowHpImage;

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

    public void Show_Flash_Effect()
    {
        _flash.GetComponent<Animator>().Play("Flash");
    }

    public void Sync_BulletCount_UI(int ammo) => _revolverUI.Ammo = ammo;
    public void Show_Revolver_Fire_Effect() => _revolverUI.FireBullet();
    public void Show_Revolver_Reload_Effect(bool isReloadAll) => _revolverUI.ReloadBullet(isReloadAll);

    public void ShowLowHP() => _lowHpImage.GetComponent<Animator>().Play("LowHP");

    public void SyncReserveAmmoUI(int ammo) => _reserveAmmoUI.text = ammo.ToString();
    public void ShowGameOver() => _gameOverPanel.SetActive(true);

    public void RetryClick() => SceneManager.LoadScene(0);
    public void ExitClick() => Application.Quit();
}
