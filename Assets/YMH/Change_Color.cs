using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public Image CrossHair;
    public TMP_Text AmmoCount;
    public Camera mainCamera;

    void Update()
    {
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(CrossHair.rectTransform, CrossHair.rectTransform.position, mainCamera, out worldPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            print(hit.collider.gameObject.name);
            SpriteRenderer object2D = hit.collider.GetComponentInChildren<SpriteRenderer>();

            if (object2D != null)
            {
                if (CrossHair.color == object2D.color) ChangeCrossHairColor(Color.yellow);
            }
        }
        else ChangeCrossHairColor(Color.white);
    }

    void ChangeCrossHairColor(Color color)
    {
        AmmoCount.color = color;
        CrossHair.color = color;
    }
}
