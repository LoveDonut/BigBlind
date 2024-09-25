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
}
