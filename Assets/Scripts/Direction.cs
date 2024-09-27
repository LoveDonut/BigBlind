using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Direction : MonoBehaviour
{
    public static Direction Instance { get; private set; }

    [Header("Direction_UI")]
    [SerializeField] GameObject Flash;

    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform CrossHair;
    [SerializeField] float smoothness = 0.1f;

    [Header("Revolver_UI")]
    [SerializeField] Revolver_UI RevolverUI;

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            CrossHair.position = Vector2.Lerp(CrossHair.position, mousePosition, smoothness);
        }
    }

    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Show_Flash_Effect()
    {
        Flash.GetComponent<Animator>().Play("Flash");
    }

    public void Sync_BulletCount_UI(int ammo) => RevolverUI._ammo = ammo;
    public void Show_Revolver_Fire_Effect() => RevolverUI.FireBullet();
    public void Show_Revolver_Reload_Effect(bool isReloadAll) => RevolverUI.ReloadBullet(isReloadAll);

}
