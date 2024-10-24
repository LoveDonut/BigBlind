using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player.transform.position = transform.GetChild(0).position;
        }
       else if(Input.GetKeyDown(KeyCode.X))
        {
            player.transform.position = transform.GetChild(1).position;
        }
    }
}
