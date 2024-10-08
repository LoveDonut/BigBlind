using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSilhouette : MonoBehaviour
{
    [SerializeField] float Destroy_Time = 10;
    float t_Destroy = 0, alpha;
    MeshRenderer meshRenderer;
    Color myColor;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        myColor = meshRenderer.material.color;
    }

    private void FixedUpdate()
    {
        UpdateWaveColor();
    }

    void UpdateWaveColor()
    {
        t_Destroy += Time.fixedDeltaTime;
        alpha = myColor.a * (1 - (t_Destroy / Destroy_Time));

        Color fillColorWithAlpha = new Color(myColor.r, myColor.g, myColor.b, alpha);
        meshRenderer.material.color = fillColorWithAlpha;

        if (alpha <= 0) Destroy(gameObject);
    }

}
