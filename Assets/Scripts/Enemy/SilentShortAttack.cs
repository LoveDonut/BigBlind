using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyState;

public class SilentShortAttack : ShortAttack
{
    #region PublicMethods
    public override void EndSleep()
    {
    }
    public override void InitChase()
    {
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();

        ResetReadyBeatCount();

        // start move if no player in attack range
        if (!IsInAttackRange())
        {
            if (enemyMovement != null)
            {
                enemyMovement.StartMove();
            }
        }
    }

    public override void EndChase()
    {
        InitWave();
    }

    public override void EndAttack()
    {
        base.EndAttack();
        WaveManager waveManager;
        if (TryGetComponent<WaveManager>(out waveManager))
        {
            waveManager.StopWave();
        }
    }

    #endregion

}
