using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_LongRange : Enemy
{
    [SerializeField] GameObject _bulletPrefab;

    public void Fire()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimPos.z = 0f;
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + (Vector3)DirectionToPlayer(), Quaternion.LookRotation(DirectionToPlayer().normalized));

        Destroy(bullet, 3f);
    }
}
