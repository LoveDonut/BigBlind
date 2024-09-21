using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BulletSenseManagement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Debug.Log("collide with enemy!");
            Animator enemyAnimator;
            if(collision.TryGetComponent<Animator>(out enemyAnimator))
            {
                enemyAnimator.SetBool("IsDead", true);
            }
            Destroy(gameObject);
        }
    }
}
