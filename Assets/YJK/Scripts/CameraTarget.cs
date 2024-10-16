using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Made By JK3WN
public class CameraTarget : MonoBehaviour
{
    #region References
    [Header("Reference")]
    [SerializeField] Camera _cam;
    [SerializeField] Transform _player;
    #endregion

    [Space]
    [SerializeField] float _threshold;
    Vector3 _mousePos;

    // Update is called once per frame
    void Update()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*
        targetPos = (player.position + mousePos) / 2f;
        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);
        */
        Vector3 direction = _mousePos - _player.position;
        if(direction.magnitude > _threshold)
        {
            direction = direction.normalized * _threshold;
        }
        this.transform.position = _player.position + direction;
    }

    void OnLook(InputValue value)
    {
        //mousePos = cam.ScreenToWorldPoint(value.Get<Vector2>()); // Change it to use NIS
    }
}
