using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class Explosion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamage damageable = collision.gameObject.GetComponent<IDamage>();
        if(damageable != null)
        {
            damageable.GetDamaged((collision.transform.position - transform.position).normalized, gameObject);
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
