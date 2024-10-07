using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject Player;

    private int _bloodIndex = 0;

    private void Awake()
    {
        Instance = this;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public int Set_New_Blood_Index() { return _bloodIndex++; }

    public void AdjustBPM(bool isIncrease)
    {
        Player.GetComponent<WaveManager>().SetBPM(isIncrease);
    }
}
