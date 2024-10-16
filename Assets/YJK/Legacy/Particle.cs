using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float DestroyTime = 5f;

    private void Start()
    {
        Invoke("DelayedDestroy", DestroyTime);
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
