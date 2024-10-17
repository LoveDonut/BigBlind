using EnemyState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by Daehui
public class EMP : MonoBehaviour
{
    #region PrivateVariables
    [SerializeField] float _effectRadius = 3f;

    WaveManager waveManager;

    Coroutine _waveCoroutine;
    #endregion

    #region PublicVariables
    public float EMPDuration = 2f;
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, _effectRadius);
    //}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ParalyzeBoomBox();
        }
    }

    public void ParalyzeBoomBox()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _effectRadius, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            if (hit.TryGetComponent<WaveManager>(out waveManager))
            {
                Debug.Log("Stop Wave!!");
                waveManager.StopWave();
                waveManager.RestartWave(hit, EMPDuration);
            }

            //RaycastHit2D wallHit = Physics2D.Raycast(transform.position, hit.transform.position - transform.position, _effectRadius, LayerMask.GetMask("Wall", "Box"));
            //if (wallHit.collider == null)
            //{
            //}
        }
    }
}
