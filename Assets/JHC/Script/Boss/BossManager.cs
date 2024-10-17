using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public GameObject Boss;

    private void Update()
    {
        if (Boss == null)
        {
            Destroy(gameObject);
        }
    }
}
