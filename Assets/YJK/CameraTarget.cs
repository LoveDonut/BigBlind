using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform player;
    [SerializeField] float threshold;
    public Vector3 mousePos;
    public Vector3 targetPos;

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*
        targetPos = (player.position + mousePos) / 2f;
        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);
        */
        Vector3 direction = mousePos - player.position;
        if(direction.magnitude > threshold)
        {
            direction = direction.normalized * threshold;
        }
        this.transform.position = player.position + direction;
    }

    void OnLook(InputValue value)
    {
        //mousePos = cam.ScreenToWorldPoint(value.Get<Vector2>()); // Change it to use NIS
    }
}
