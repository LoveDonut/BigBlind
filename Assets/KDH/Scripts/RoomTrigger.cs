using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Enemy"))
                {
                    child.gameObject.SetActive(true);
                    if (child.gameObject.TryGetComponent<EnemyPatrol>(out EnemyPatrol enemyPatrol))
                    {
                        enemyPatrol.IsFindPlayer = true;
                    }
                }
            }
        }
    }
}
