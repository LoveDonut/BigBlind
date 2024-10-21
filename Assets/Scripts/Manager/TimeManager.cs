using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// made by KimDaehui
public class TimeManager : MonoBehaviour
{
    #region PrivateVariables
    [SerializeField] float BPM = 135;
    Coroutine _coroutine;
    bool _isSlowed = false;
    #endregion

    #region PublicVariables
    public static TimeManager Instance;
    [SerializeField] public Queue<WaveManager> _waveManagers;
    [SerializeField] SpriteRenderer _playerSprite;
    public bool IsClickRight;
    #endregion

    #region PrivateMethods
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if(SoundManager.Instance != null) BPM = SoundManager.Instance.BPM;
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
        PlayerShoot playershoot = FindAnyObjectByType<PlayerShoot>();
        float bpm = BPM;

        if (playershoot != null)
        {
            bpm = BPM / playershoot.ShootBPMDivider;
        }

        while (true)
        {
            if (playershoot != null)
            {
                yield return new WaitForSeconds(24/ bpm);
                _playerSprite.color = Color.yellow;
                yield return new WaitForSeconds(6 / bpm);
                _playerSprite.color = Color.white;
                yield return new WaitForSeconds((24 - (60 * playershoot.ShootBPMBufferMultiplier >= 24 ? 24 : 60 * playershoot.ShootBPMBufferMultiplier)) / bpm);
                IsClickRight = true;
                yield return new WaitForSeconds((60 * playershoot.ShootBPMBufferMultiplier >= 24 ? 24 : 60 * playershoot.ShootBPMBufferMultiplier) / bpm);
                _playerSprite.color = Color.red;
                yield return new WaitForSeconds(6 / bpm);
                _playerSprite.color = Color.white;
                IsClickRight = false;
                if (!playershoot.ShouldClickShootByBeat)
                {
                    if (playershoot.IsShootable)
                    {
                        playershoot.Shoot();
                        playershoot.IsShootable = false;
                    }
                }
                bpm = BPM / playershoot.ShootBPMDivider;
            }


            while (_waveManagers.Count > 0)
            {

                WaveManager enemyWave = _waveManagers.Dequeue();
                if (enemyWave != null)
                {
                    if(!enemyWave.WillBeOff)
                    {
                        enemyWave.StartWaveByBeat();
                    }
                    else
                    {
                        enemyWave.WillBeOff = false;
                    }
                }
            }
        }
    }

    IEnumerator ResetTimeScale(float slowDownDuration)
    {
        yield return new WaitForSecondsRealtime(slowDownDuration);

        // recover timescale
        UnityEngine.Time.timeScale = 1f;
        UnityEngine.Time.fixedDeltaTime = 0.02f; // default fixedDeltaTime is 0.02f
        _isSlowed = false;
    }
    #endregion

    #region PublicMethods
    public void DoSlowMotion(float slowDownFactor, float slowDownDuration)
    {
        if (_isSlowed) return;
        // slow down
        UnityEngine.Time.timeScale = slowDownFactor;
        UnityEngine.Time.fixedDeltaTime = slowDownFactor * 0.02f;

        _coroutine = StartCoroutine(ResetTimeScale(slowDownDuration));
        _isSlowed = true;
    }
    #endregion
}
