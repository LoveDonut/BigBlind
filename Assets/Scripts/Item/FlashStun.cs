using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

// made by Daehui
public class FlashStun : MonoBehaviour
{
    #region PrivateVariables
    [SerializeField] float _effectRadius = 3f;
    Collider2D[] hits = new Collider2D[20];
    #endregion

    #region PublicVariables
    public float StunDuration = 2f;
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, _effectRadius);
    //}

    public void Stun()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, _effectRadius, hits, LayerMask.GetMask("Enemy"));

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            // check if there is no wall between enemy and player
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, hit.transform.position - transform.position, _effectRadius, LayerMask.GetMask("Wall", "Box"));

            if(wallHit.collider == null)
            {
                EnemyMovement enemyMovement;
                if(hit.TryGetComponent<EnemyMovement>(out enemyMovement))
                {
                    if (enemyMovement.CurrentState != null)
                    {
                        StunState stunState = new StunState();
                        stunState.ElapsedTime = StunDuration;
                        enemyMovement.CurrentState.SwitchState(hit.gameObject, ref enemyMovement.CurrentState, stunState);
                    }

                }
            }
        }
    }
}
