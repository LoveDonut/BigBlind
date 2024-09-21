using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    public static Direction Instance { get; private set; }

    [Header("Direction_UI")]
    [SerializeField] GameObject Flash;

    private void Awake()
    {
        Instance = this;
    }

    public void Show_Flash_Effect()
    {
        Flash.GetComponent<Animator>().Play("Flash");
    }
}
