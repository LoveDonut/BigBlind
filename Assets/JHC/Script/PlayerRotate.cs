using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform _spriteObj;

    CameraTarget _cameraTarget;
    
    void Awake()
    {
        _cameraTarget = FindAnyObjectByType<CameraTarget>();
    }

    public void Rotate()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(aimPos.y, aimPos.x) * Mathf.Rad2Deg - 270f ;
        _spriteObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
