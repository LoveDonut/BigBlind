using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class AmmoAdd : MonoBehaviour
{
    [SerializeField] int _ammo = 6;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerShoot>().AddReserveAmmo(_ammo);
            Destroy(gameObject);
        }
    }
}
