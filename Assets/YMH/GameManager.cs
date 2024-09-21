using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int BloodIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public int Set_New_Blood_Index() { return BloodIndex++; }
}
