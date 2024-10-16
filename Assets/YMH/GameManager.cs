using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int _bloodIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public int Set_New_Blood_Index() { return _bloodIndex++; }

    // made by KimDaehui
    public void RestartStage()
    {

        // recover timescale
        UnityEngine.Time.timeScale = 1f;
        UnityEngine.Time.fixedDeltaTime = 0.02f; // default fixedDeltaTime is 0.02f
    }
}
