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

        if (playerMovement != null)
        {
        }

        // stop player tackle
        if (playerMovement.CurrentState is PlayerState.ShortAttackState)
        {
            PlayerState.ShortAttackState shortAttackState = (PlayerState.ShortAttackState)playerMovement.CurrentState;

            shortAttackState.IsPrevented = true;
        }

        // knockback player
        if (playerHealth != null)
        {
            playerHealth.GetDamaged(-knockBackDirection * 20f, 0, false);
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
