using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    Coroutine coroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DoSlowMotion(float slowDownFactor, float slowDownDuration)
    {
        // slow down
        UnityEngine.Time.timeScale = slowDownFactor;
        UnityEngine.Time.fixedDeltaTime = slowDownFactor * 0.02f;

        coroutine = StartCoroutine(ResetTimeScale(slowDownDuration));
    }

    IEnumerator ResetTimeScale(float slowDownDuration)
    {
        yield return new WaitForSecondsRealtime(slowDownDuration);

        // recover timescale
        UnityEngine.Time.timeScale = 1f;
        UnityEngine.Time.fixedDeltaTime = 0.02f; // default fixedDeltaTime is 0.02f
    }
}
