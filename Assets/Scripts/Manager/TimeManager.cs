using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public class TimeManager : MonoBehaviour
{
    #region PrivateVariables
    [SerializeField] float BPM = 135;
    Coroutine _coroutine;
    #endregion

    #region PublicVariables
    public static TimeManager Instance;
    [SerializeField] public Queue<WaveManager> _waveManagers;
    #endregion

    #region PrivateMethods
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _waveManagers = new Queue<WaveManager>();
        StartCoroutine(TurnOnEnemiesWaveByBeat());
    }

    IEnumerator TurnOnEnemiesWaveByBeat()
    {
        while (true)
        {
            while (_waveManagers.Count > 0)
            {
                WaveManager enemyWave = _waveManagers.Dequeue();
                if (enemyWave != null)
                {
                    enemyWave.StartWaveByBeat();
                }
            }
            yield return new WaitForSeconds(60 / BPM);
        }
    }

    IEnumerator ResetTimeScale(float slowDownDuration)
    {
        yield return new WaitForSecondsRealtime(slowDownDuration);

        // recover timescale
        UnityEngine.Time.timeScale = 1f;
        UnityEngine.Time.fixedDeltaTime = 0.02f; // default fixedDeltaTime is 0.02f
    }
    #endregion

    #region PublicMethods
    public void DoSlowMotion(float slowDownFactor, float slowDownDuration)
    {
        // slow down
        UnityEngine.Time.timeScale = slowDownFactor;
        UnityEngine.Time.fixedDeltaTime = slowDownFactor * 0.02f;

        _coroutine = StartCoroutine(ResetTimeScale(slowDownDuration));
    }
    #endregion
}
