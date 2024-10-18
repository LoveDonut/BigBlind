using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class Explosion : MonoBehaviour
{
    float _radius = 3f;

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius);
        foreach (Collider2D hit in hits)
        {
            IDamage damageable = hit.gameObject.GetComponent<IDamage>();
            if (damageable != null)
            {
                Vector2 directionToTarget = (hit.transform.position - transform.position).normalized;
                float distanceToTarget = Vector2.Distance(transform.position, directionToTarget);
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, LayerMask.GetMask("Wall", "Player", "Glass", "Box", "Enemy"));
                Debug.Log(rayHit.collider);
                if(rayHit.collider == hit) damageable.GetDamaged((hit.transform.position - transform.position).normalized, gameObject);
            }
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
