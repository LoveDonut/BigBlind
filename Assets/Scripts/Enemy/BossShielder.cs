using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public class BossShielder : MonoBehaviour, IKnockback
{
    void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        WaveManager waveManager = GetComponentInParent<WaveManager>();

        while (waveManager == null || TimeManager.Instance._waveManagers == null)
        {
            yield return new WaitForEndOfFrame();
        }

        waveManager.EnqueueWaveForPlayingByBeat();
    }

    public void Knockback(Vector2 knockBackDirection, GameObject gameObject)
    {        
        PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();

        // stop player tackle
        if (playerMovement.CurrentState is PlayerState.ShortAttackState)
        {
            PlayerState.ShortAttackState shortAttackState = (PlayerState.ShortAttackState)playerMovement.CurrentState;

            shortAttackState.IsPrevented = true;
        }

        // knockback player
        if (playerHealth != null)
        {
            EnemyMovement enemyMovement = GetComponentInParent<EnemyMovement>();
            playerHealth.GetDamaged(-knockBackDirection * 20f, enemyMovement != null ? enemyMovement.gameObject : gameObject,0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IParriable parriable;
        if (collision.gameObject.TryGetComponent<IParriable>(out parriable))
        {
            Debug.Log($"parriable! I'm {gameObject.name}");
            parriable.IsParried = true;
        }
    }
}
